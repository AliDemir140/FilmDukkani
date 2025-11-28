using Application.DTOs.MemberMovieListDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MemberMovieListServiceManager
    {
        private readonly IMemberMovieListRepository _memberMovieListRepository;
        private readonly IMemberMovieListItemRepository _memberMovieListItemRepository;

        public MemberMovieListServiceManager(
            IMemberMovieListRepository memberMovieListRepository,
            IMemberMovieListItemRepository memberMovieListItemRepository)
        {
            _memberMovieListRepository = memberMovieListRepository;
            _memberMovieListItemRepository = memberMovieListItemRepository;
        }

        // Bir üyenin listelerini getir
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

        // Yeni liste oluştur
        public async Task<int> CreateListAsync(CreateMemberMovieListDto dto)
        {
            var list = new MemberMovieList
            {
                MemberId = dto.MemberId,
                Name = dto.Name
            };

            await _memberMovieListRepository.AddAsync(list);

            return list.ID; // ID otomatik atanır
        }

        // Bir listenin içindeki film item'larını getir
        public async Task<List<MemberMovieListItemDto>> GetListItemsAsync(int listId)
        {
            var items = await _memberMovieListItemRepository.GetAllAsync(i => i.MemberMovieListId == listId);

            return items
                .Select(i => new MemberMovieListItemDto
                {
                    Id = i.ID,
                    MemberMovieListId = i.MemberMovieListId,
                    MovieId = i.MovieId,
                    Priority = i.Priority,
                    AddedDate = i.AddedDate
                })
                .ToList();
        }

        // Aynı filmi aynı listeye iki kez eklemeyi önlemek için kontrol
        private async Task<bool> IsMovieAlreadyInList(int listId, int movieId)
        {
            var items = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == listId && i.MovieId == movieId);

            return items.Any();
        }

        // Listeye film ekle
        public async Task<bool> AddItemToListAsync(CreateMemberMovieListItemDto dto)
        {
            // Aynı film daha önce eklenmiş mi kontrolü
            if (await IsMovieAlreadyInList(dto.MemberMovieListId, dto.MovieId))
                return false; // Controller bunu "Film zaten listede" diye dönecek

            var item = new MemberMovieListItem
            {
                MemberMovieListId = dto.MemberMovieListId,
                MovieId = dto.MovieId,
                Priority = dto.Priority,
                AddedDate = DateTime.Now
            };

            await _memberMovieListItemRepository.AddAsync(item);
            return true;
        }

        // Min 5 film kontrolü
        public async Task<bool> HasMinimumItemsAsync(int listId, int minimumCount = 5)
        {
            var items = await _memberMovieListItemRepository
                .GetAllAsync(i => i.MemberMovieListId == listId);

            return items.Count >= minimumCount;
        }

        // Liste adını güncelle
        public async Task<bool> UpdateListNameAsync(UpdateMemberMovieListNameDto dto)
        {
            var list = await _memberMovieListRepository.GetByIdAsync(dto.Id);
            if (list == null)
                return false;

            list.Name = dto.Name;
            await _memberMovieListRepository.UpdateAsync(list);
            return true;
        }

        // Liste item'ını sil
        public async Task<bool> DeleteItemAsync(int itemId)
        {
            var item = await _memberMovieListItemRepository.GetByIdAsync(itemId);
            if (item == null)
                return false;

            await _memberMovieListItemRepository.DeleteAsync(item);
            return true;
        }

        // Liste item'ının önceliğini güncelle
        public async Task<bool> UpdateItemPriorityAsync(UpdateMemberMovieListItemPriorityDto dto)
        {
            var item = await _memberMovieListItemRepository.GetByIdAsync(dto.Id);
            if (item == null)
                return false;

            item.Priority = dto.Priority;
            await _memberMovieListItemRepository.UpdateAsync(item);
            return true;
        }

    }
}
