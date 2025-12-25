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

        public MemberMovieListServiceManager(
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository,
            IMovieRepository moviesRepository)
        {
            _memberMovieListRepository = memberMovieListRepository;
            _memberMovieListItemRepository = memberMovieListItemRepository;
            _moviesRepository = moviesRepository;
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

        // ✅ KURAL: aynı isimden liste oluşturulamasın (case-insensitive + trim)
        public async Task<int> CreateListAsync(CreateMemberMovieListDto dto)
        {
            var name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
                return 0;

            var existing = await _memberMovieListRepository.GetAllAsync(x =>
                x.MemberId == dto.MemberId &&
                x.Name.ToLower() == name.ToLower()
            );

            if (existing.Any())
                return 0;

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
                : new List<Domain.Entities.Movie>();

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

            // ✅ aynı isim kontrolü
            var dup = await _memberMovieListRepository.GetAllAsync(x =>
                x.MemberId == list.MemberId &&
                x.ID != list.ID &&
                x.Name.ToLower() == newName.ToLower()
            );

            if (dup.Any())
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

        // ✅ Up/Down swap
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
    }
}
