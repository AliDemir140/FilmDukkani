using Application.DTOs.CourierDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class CourierServiceManager
    {
        private readonly ICourierRepository _courierRepository;

        public CourierServiceManager(ICourierRepository courierRepository)
        {
            _courierRepository = courierRepository;
        }

        public async Task<List<CourierDto>> GetAllAsync(bool onlyActive = false)
        {
            var list = onlyActive
                ? await _courierRepository.GetAllAsync(x => x.IsActive)
                : await _courierRepository.GetAllAsync();

            return list
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .Select(x => new CourierDto
                {
                    Id = x.ID,
                    FullName = $"{x.FirstName} {x.LastName}",
                    Phone = x.Phone,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public async Task<int> CreateAsync(CreateCourierDto dto)
        {
            var entity = new Courier
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                IsActive = dto.IsActive
            };

            await _courierRepository.AddAsync(entity);
            return entity.ID;
        }

        public async Task<bool> UpdateAsync(UpdateCourierDto dto)
        {
            var entity = await _courierRepository.GetByIdAsync(dto.Id);
            if (entity == null) return false;

            entity.FirstName = dto.FirstName.Trim();
            entity.LastName = dto.LastName.Trim();
            entity.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            entity.IsActive = dto.IsActive;

            await _courierRepository.UpdateAsync(entity);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _courierRepository.GetByIdAsync(id);
            if (entity == null) return false;

            await _courierRepository.DeleteAsync(entity);
            return true;
        }
    }
}
