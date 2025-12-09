using Application.DTOs.AwardDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class AwardServiceManager
    {
        private readonly IAwardRepository _awardRepository;

        public AwardServiceManager(IAwardRepository awardRepository)
        {
            _awardRepository = awardRepository;
        }

        public async Task<List<AwardDto>> GetAwardsAsync()
        {
            var awards = await _awardRepository.GetAllAsync();

            return awards
                .Select(a => new AwardDto
                {
                    Id = a.ID,
                    AwardName = a.Name,
                    Organization = a.Organization,
                    CreatedDate = a.CreatedDate,
                    ModifiedDate = a.ModifiedDate
                })
                .ToList();
        }

        public async Task AddAwardAsync(CreateAwardDto dto)
        {
            var award = new Award
            {
                Name = dto.AwardName,
                Organization = dto.Organization
            };

            await _awardRepository.AddAsync(award);
        }

        public async Task<UpdateAwardDto?> GetAwardAsync(int id)
        {
            var award = await _awardRepository.GetByIdAsync(id);
            if (award == null)
                return null;

            return new UpdateAwardDto
            {
                Id = award.ID,
                AwardName = award.Name,
                Organization = award.Organization
            };
        }

        public async Task<bool> UpdateAwardAsync(UpdateAwardDto dto)
        {
            var award = await _awardRepository.GetByIdAsync(dto.Id);
            if (award == null)
                return false;

            award.Name = dto.AwardName;
            award.Organization = dto.Organization;

            await _awardRepository.UpdateAsync(award);
            return true;
        }

        public async Task<bool> DeleteAwardAsync(int id)
        {
            var award = await _awardRepository.GetByIdAsync(id);
            if (award == null)
                return false;

            await _awardRepository.DeleteAsync(award);
            return true;
        }
    }
}
