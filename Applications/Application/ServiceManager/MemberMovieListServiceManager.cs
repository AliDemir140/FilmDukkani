using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.MemberMovieListDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MemberMovieListServiceManager
    {
        private readonly IMemberMovieListRepository _memberMovieListRepository;
        private readonly IMemberMovieListItemRepository _memberMovieListItemRepository;
        private readonly IMovieRepository _moviesRepository;
        private readonly IDeliveryRequestRepository _deliveryRequestRepository;

        public MemberMovieListServiceManager(
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository,
            IMovieRepository moviesRepository,
            IDeliveryRequestRepository deliveryRequestRepository)
        {
            _memberMovieListRepository = memberMovieListRepository;
            _memberMovieListItemRepository = memberMovieListItemRepository;
            _moviesRepository = moviesRepository;
            _deliveryRequestRepository = deliveryRequestRepository;
        }

        private async Task<MemberMovieList?> GetListAsync(int listId)
        {
            if (listId <= 0) return null;
            return await _memberMovieListRepository.GetByIdAsync(listId);
        }

        private async Task<bool> IsListLockedAsync(int listId)
        {
            var list = await GetListAsync(listId);
            if (list == null) return false;

            return await _deliveryRequestRepository.HasActiveRequestForListAsync(list.MemberId, listId);
        }

        public async Task<List<MemberMovieListDto>> GetListsByMemberAsync(int memberId)
        {
            var lists = await _memberMovieListRepository.GetAllAsync(l => l.MemberId == memberId);

            return lists
                .OrderByDescending(x => x.ID)
                .Select(l => new MemberMovieListDto
                {
                    Id = l.ID,
                    MemberId = l.MemberId,
                    Name = l.Name
                })
                .ToList();
        }

        public async Task<int> CreateListAsync(CreateMemberMovieListDto dto)
        {
            if (dto == null) return 0;
            if (dto.MemberId <= 0) return 0;

            var name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            var exists = await _memberMovieListRepository.ExistsByNameAsync(dto.MemberId, name);
            if (exists)
                return -1;

            var list = new MemberMovieList
            {
                MemberId = dto.MemberId,
                Name = name,
                Items = new List<MemberMovieListItem>()
            };

            await _memberMovieListRepository.AddAsync(list);
            return list.ID;
        }

        public async Task<List<MemberMovieListItemDto>> GetListItemsAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i =>
                i.MemberMovieListId == listId && i.IsReserved == false);

            var movieIds = items.Select(x => x.MovieId).Distinct().ToList();

            var movies = movieIds.Any()
                ? await _moviesRepository.GetAllAsync(m => movieIds.Contains(m.ID))
                : new List<Movie>();

            var movieMap = movies.ToDictionary(m => m.ID, m => m.Title);

            return items
                .OrderBy(i => i.Priority)
                .ThenBy(i => i.AddedDate)
                .Select(i => new MemberMovieListItemDto
                {
                    Id = i.ID,
                    MemberMovieListId = i.MemberMovieListId,
                    MovieId = i.MovieId,
                    MovieTitle = movieMap.ContainsKey(i.MovieId) ? movieMap[i.MovieId] : null,
                    Priority = i.Priority,
                    AddedDate = i.AddedDate
                })
                .ToList();
        }

        public async Task<List<MemberMovieListItemDto>> GetListItemsAllAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i =>
                i.MemberMovieListId == listId);

            var movieIds = items.Select(x => x.MovieId).Distinct().ToList();

            var movies = movieIds.Any()
                ? await _moviesRepository.GetAllAsync(m => movieIds.Contains(m.ID))
                : new List<Movie>();

            var movieMap = movies.ToDictionary(m => m.ID, m => m.Title);

            return items
                .OrderBy(i => i.Priority)
                .ThenBy(i => i.AddedDate)
                .ThenBy(i => i.ID)
                .Select(i => new MemberMovieListItemDto
                {
                    Id = i.ID,
                    MemberMovieListId = i.MemberMovieListId,
                    MovieId = i.MovieId,
                    MovieTitle = movieMap.ContainsKey(i.MovieId) ? movieMap[i.MovieId] : null,
                    Priority = i.Priority,
                    AddedDate = i.AddedDate
                })
                .ToList();
        }

        private async Task<bool> IsMovieAlreadyInList(int listId, int movieId)
        {
            var items = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == listId && i.MovieId == movieId);

            return items.Any();
        }

        public async Task<int> AddItemToListAsync(CreateMemberMovieListItemDto dto)
        {
            if (dto == null) return 0;
            if (dto.MemberMovieListId <= 0 || dto.MovieId <= 0) return 0;

            if (await IsListLockedAsync(dto.MemberMovieListId))
                return -1;

            if (await IsMovieAlreadyInList(dto.MemberMovieListId, dto.MovieId))
                return 0;

            var allItems = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == dto.MemberMovieListId);

            int nextPriority = allItems.Any()
                ? allItems.Max(x => x.Priority) + 1
                : 1;

            int pr = (dto.Priority.HasValue && dto.Priority.Value > 0)
                ? dto.Priority.Value
                : nextPriority;

            var item = new MemberMovieListItem
            {
                MemberMovieListId = dto.MemberMovieListId,
                MovieId = dto.MovieId,
                Priority = pr,
                AddedDate = DateTime.Now
            };

            await _memberMovieListItemRepository.AddAsync(item);
            return 1;
        }


        public async Task<bool> HasMinimumItemsAsync(int listId, int minimumCount = 5)
        {
            var items = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == listId);

            return items.Count >= minimumCount;
        }

        public async Task<int> GetItemCountAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
            return items.Count;
        }

        public async Task<int> UpdateListNameAsync(UpdateMemberMovieListNameDto dto)
        {
            if (dto == null || dto.Id <= 0) return 0;

            var list = await _memberMovieListRepository.GetByIdAsync(dto.Id);
            if (list == null) return 0;

            if (await IsListLockedAsync(list.ID))
                return -1;

            var newName = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newName))
                return 0;

            var exists = await _memberMovieListRepository.ExistsByNameAsync(list.MemberId, newName);
            if (exists && !string.Equals(list.Name?.Trim(), newName, StringComparison.OrdinalIgnoreCase))
                return -2;

            list.Name = newName;
            await _memberMovieListRepository.UpdateAsync(list);
            return 1;
        }

        public async Task<int> DeleteItemAsync(int itemId)
        {
            if (itemId <= 0) return 0;

            var item = await _memberMovieListItemRepository.GetByIdAsync(itemId);
            if (item == null) return 0;

            if (await IsListLockedAsync(item.MemberMovieListId))
                return -1;

            await _memberMovieListItemRepository.DeleteAsync(item);
            return 1;
        }

        public async Task<int> UpdateItemPriorityAsync(UpdateMemberMovieListItemPriorityDto dto)
        {
            if (dto == null || dto.Id <= 0) return 0;

            var item = await _memberMovieListItemRepository.GetByIdAsync(dto.Id);
            if (item == null) return 0;

            if (await IsListLockedAsync(item.MemberMovieListId))
                return -1;

            if (dto.Priority <= 0) dto.Priority = 1;

            item.Priority = dto.Priority;
            await _memberMovieListItemRepository.UpdateAsync(item);
            return 1;
        }

        public async Task<bool> ListExistsAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            return list != null;
        }

        public async Task<int> ReorderItemsAsync(int listId, List<int> orderedItemIds)
        {
            if (listId <= 0) return 0;

            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null) return 0;

            if (await IsListLockedAsync(listId))
                return -1;

            orderedItemIds ??= new List<int>();

            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
            if (!items.Any()) return 0;

            var itemMap = items.ToDictionary(x => x.ID, x => x);

            int pr = 1;
            foreach (var itemId in orderedItemIds)
            {
                if (!itemMap.ContainsKey(itemId))
                    continue;

                itemMap[itemId].Priority = pr;
                await _memberMovieListItemRepository.UpdateAsync(itemMap[itemId]);
                pr++;
            }

            return 1;
        }

        private async Task NormalizePrioritiesAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
            if (items == null || !items.Any())
                return;

            var ordered = items
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.AddedDate)
                .ThenBy(x => x.ID)
                .ToList();

            for (int i = 0; i < ordered.Count; i++)
            {
                var newPriority = i + 1;
                if (ordered[i].Priority != newPriority)
                {
                    ordered[i].Priority = newPriority;
                    await _memberMovieListItemRepository.UpdateAsync(ordered[i]);
                }
            }
        }

        public async Task<int> MoveItemAsync(int listId, int itemId, string direction)
        {
            if (listId <= 0 || itemId <= 0) return 0;

            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null) return 0;

            if (await IsListLockedAsync(listId))
                return -1;

            await NormalizePrioritiesAsync(listId);

            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

            var ordered = items
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.AddedDate)
                .ThenBy(x => x.ID)
                .ToList();

            if (!ordered.Any())
                return 0;

            var index = ordered.FindIndex(x => x.ID == itemId);
            if (index < 0)
                return 0;

            direction = (direction ?? "").Trim().ToLower();

            if (direction == "up")
            {
                if (index == 0) return 1;

                var current = ordered[index];
                var prev = ordered[index - 1];

                int tmp = current.Priority;
                current.Priority = prev.Priority;
                prev.Priority = tmp;

                await _memberMovieListItemRepository.UpdateAsync(prev);
                await _memberMovieListItemRepository.UpdateAsync(current);
                return 1;
            }

            if (direction == "down")
            {
                if (index == ordered.Count - 1) return 1;

                var current = ordered[index];
                var next = ordered[index + 1];

                int tmp = current.Priority;
                current.Priority = next.Priority;
                next.Priority = tmp;

                await _memberMovieListItemRepository.UpdateAsync(next);
                await _memberMovieListItemRepository.UpdateAsync(current);
                return 1;
            }

            return 0;
        }

        public async Task<int> ClearListItemsAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null)
                return 0;

            if (await IsListLockedAsync(listId))
                return -1;

            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

            foreach (var it in items)
                await _memberMovieListItemRepository.DeleteAsync(it);

            return 1;
        }

        public async Task<int> DeleteListAsync(int listId)
        {
            if (listId <= 0) return 0;

            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null) return 0;

            if (await IsListLockedAsync(listId))
                return -1;

            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
            foreach (var it in items)
                await _memberMovieListItemRepository.DeleteAsync(it);

            await _memberMovieListRepository.DeleteAsync(list);
            return 1;
        }

        public async Task<int> ClearAllNonOrderedListsAsync(int memberId)
        {
            var lists = await _memberMovieListRepository.GetAllAsync(l => l.MemberId == memberId);
            int cleared = 0;

            foreach (var list in lists)
            {
                var locked = await _deliveryRequestRepository.HasActiveRequestForListAsync(memberId, list.ID);
                if (locked)
                    continue;

                var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == list.ID);
                if (!items.Any())
                    continue;

                foreach (var it in items)
                    await _memberMovieListItemRepository.DeleteAsync(it);

                cleared++;
            }

            return cleared;
        }

        public async Task<bool> IsListLockedPublicAsync(int listId)
        {
            return await IsListLockedAsync(listId);
        }
    }
}
