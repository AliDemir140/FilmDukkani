using Application.DTOs.ShelfDTOs;
using Application.Repositories;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ServiceManager
{
    public class ShelfServiceManager
    {
        private readonly IShelfRepository _shelfRepository;

        public ShelfServiceManager(IShelfRepository shelfRepository)
        {
            _shelfRepository = shelfRepository;
        }

        // Tüm rafları listele
        public async Task<List<ShelfDto>> GetShelvesAsync()
        {
            var shelves = await _shelfRepository.GetAllAsync();

            return shelves
                .Select(s => new ShelfDto
                {
                    Id = s.ID,
                    Name = s.Name,
                    LocationCode = s.LocationCode,
                    CreatedDate = s.CreatedDate,
                    ModifiedDate = s.ModifiedDate
                })
                .ToList();
        }

        // Tek raf getir (update formu vs.)
        public async Task<UpdateShelfDto?> GetShelfAsync(int id)
        {
            var shelf = await _shelfRepository.GetByIdAsync(id);
            if (shelf == null)
                return null;

            return new UpdateShelfDto
            {
                Id = shelf.ID,
                Name = shelf.Name,
                LocationCode = shelf.LocationCode
            };
        }

        // Raf ekle
        public async Task AddShelfAsync(CreateShelfDto dto)
        {
            var shelf = new Shelf
            {
                Name = dto.Name,
                LocationCode = dto.LocationCode
            };

            await _shelfRepository.AddAsync(shelf);
        }

        // Raf güncelle
        public async Task<bool> UpdateShelfAsync(UpdateShelfDto dto)
        {
            var shelf = await _shelfRepository.GetByIdAsync(dto.Id);
            if (shelf == null)
                return false;

            shelf.Name = dto.Name;
            shelf.LocationCode = dto.LocationCode;

            await _shelfRepository.UpdateAsync(shelf);
            return true;
        }

        // Raf sil
        public async Task<bool> DeleteShelfAsync(int id)
        {
            var shelf = await _shelfRepository.GetByIdAsync(id);
            if (shelf == null)
                return false;

            await _shelfRepository.DeleteAsync(shelf);
            return true;
        }
    }
}
