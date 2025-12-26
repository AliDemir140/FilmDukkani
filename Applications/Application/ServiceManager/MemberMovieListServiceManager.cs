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

        public async Task<List<MemberMovieListDto>> GetListsByMemberAsync(int memberId)
        {
            var lists = await _memberMovieListRepository.GetAllAsync(l => l.MemberId == memberId);

            return lists
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

            var name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            var exists = await _memberMovieListRepository.ExistsByNameAsync(dto.MemberId, name);
            if (exists)
                return -1;

            var list = new MemberMovieList
            {
                MemberId = dto.MemberId,
                Name = name
            };

            await _memberMovieListRepository.AddAsync(list);
            return list.ID;
        }

        public async Task<List<MemberMovieListItemDto>> GetListItemsAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

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

        private async Task<bool> IsMovieAlreadyInList(int listId, int movieId)
        {
            var items = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == listId && i.MovieId == movieId);

            return items.Any();
        }

        public async Task<bool> AddItemToListAsync(CreateMemberMovieListItemDto dto)
        {
            if (await IsMovieAlreadyInList(dto.MemberMovieListId, dto.MovieId))
                return false;

            var item = new MemberMovieListItem
            {
                MemberMovieListId = dto.MemberMovieListId,
                MovieId = dto.MovieId,
                Priority = dto.Priority <= 0 ? 1 : dto.Priority,
                AddedDate = DateTime.Now
            };

            await _memberMovieListItemRepository.AddAsync(item);
            return true;
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

        public async Task<bool> UpdateListNameAsync(UpdateMemberMovieListNameDto dto)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(dto.Id);
            if (list == null)
                return false;

            var newName = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            var exists = await _memberMovieListRepository.ExistsByNameAsync(list.MemberId, newName);
            if (exists && !string.Equals(list.Name?.Trim(), newName, StringComparison.OrdinalIgnoreCase))
                return false;

            list.Name = newName;
            await _memberMovieListRepository.UpdateAsync(list);
            return true;
        }

        public async Task<bool> DeleteItemAsync(int itemId)
        {
            var item = await _memberMovieListItemRepository.GetByIdAsync(itemId);
            if (item == null)
                return false;

            await _memberMovieListItemRepository.DeleteAsync(item);
            return true;
        }

        public async Task<bool> UpdateItemPriorityAsync(UpdateMemberMovieListItemPriorityDto dto)
        {
            var item = await _memberMovieListItemRepository.GetByIdAsync(dto.Id);
            if (item == null)
                return false;

            item.Priority = dto.Priority;
            await _memberMovieListItemRepository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> ListExistsAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            return list != null;
        }

        public async Task<bool> ReorderItemsAsync(int listId, List<int> orderedItemIds)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
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

            return true;
        }

        public async Task<bool> MoveItemAsync(int listId, int itemId, string direction)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

            var ordered = items
                .OrderBy(x => x.Priority)
                .ThenBy(x => x.AddedDate)
                .ToList();

            if (!ordered.Any())
                return false;

            var index = ordered.FindIndex(x => x.ID == itemId);
            if (index < 0)
                return false;

            direction = (direction ?? "").Trim().ToLower();

            if (direction == "up")
            {
                if (index == 0) return true;

                var current = ordered[index];
                var prev = ordered[index - 1];

                int tmp = current.Priority;
                current.Priority = prev.Priority;
                prev.Priority = tmp;

                await _memberMovieListItemRepository.UpdateAsync(prev);
                await _memberMovieListItemRepository.UpdateAsync(current);
                return true;
            }

            if (direction == "down")
            {
                if (index == ordered.Count - 1) return true;

                var current = ordered[index];
                var next = ordered[index + 1];

                int tmp = current.Priority;
                current.Priority = next.Priority;
                next.Priority = tmp;

                await _memberMovieListItemRepository.UpdateAsync(next);
                await _memberMovieListItemRepository.UpdateAsync(current);
                return true;
            }

            return false;
        }

        // ✅ Tek listeyi boşalt (itemları sil, liste kalsın)
        public async Task<bool> ClearListItemsAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null)
                return false;

            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

            foreach (var it in items)
                await _memberMovieListItemRepository.DeleteAsync(it);

            return true;
        }

        // ✅ Listeyi sil (aktif sipariş yoksa)
        // return: 0 yok, -1 aktif sipariş var, 1 başarılı
        public async Task<int> DeleteListAsync(int listId)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(listId);
            if (list == null)
                return 0;

            // aktif sipariş var mı? (memberId repo istiyor)
            var hasActive = await _deliveryRequestRepository.HasActiveRequestForListAsync(list.MemberId, listId);
            if (hasActive)
                return -1;

            // önce itemları sil
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);
            foreach (var it in items)
                await _memberMovieListItemRepository.DeleteAsync(it);

            // sonra listeyi sil
            await _memberMovieListRepository.DeleteAsync(list);
            return 1;
        }

        // ✅ Toplu temizlik: siparişe girmemiş listelerin itemlarını sil
        // dönen: kaç liste boşaltıldı
        public async Task<int> ClearAllNonOrderedListsAsync(int memberId)
        {
            var lists = await _memberMovieListRepository.GetAllAsync(l => l.MemberId == memberId);
            int cleared = 0;

            foreach (var list in lists)
            {
                var hasActive = await _deliveryRequestRepository.HasActiveRequestForListAsync(memberId, list.ID);
                if (hasActive)
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
    }
}
