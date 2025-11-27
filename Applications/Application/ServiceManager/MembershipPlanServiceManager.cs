using Application.DTOs.MembershipPlanDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MembershipPlanServiceManager
    {
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public MembershipPlanServiceManager(IMembershipPlanRepository membershipPlanRepository)
        {
            _membershipPlanRepository = membershipPlanRepository;
        }

        // TÜM PLANLARI LİSTELEME
        public async Task<List<MembershipPlanDto>> GetPlansAsync()
        {
            var plans = await _membershipPlanRepository.GetAllAsync();

            return plans
                .Select(p => new MembershipPlanDto
                {
                    Id = p.ID,
                    PlanName = p.PlanName,
                    Price = p.Price,
                    MaxMoviesPerMonth = p.MaxMoviesPerMonth,
                    MaxChangePerMonth = p.MaxChangePerMonth,
                    Description = p.Description
                })
                .ToList();
        }

        // TEK PLAN GETİRME (UPDATE İÇİN)
        public async Task<UpdateMembershipPlanDto?> GetPlan(int id)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(id);
            if (plan == null) return null;

            return new UpdateMembershipPlanDto
            {
                Id = plan.ID,
                PlanName = plan.PlanName,
                Price = plan.Price,
                MaxMoviesPerMonth = plan.MaxMoviesPerMonth,
                MaxChangePerMonth = plan.MaxChangePerMonth,
                Description = plan.Description
            };
        }

        // PLAN EKLEME
        public async Task AddPlan(CreateMembershipPlanDto dto)
        {
            var plan = new MembershipPlan
            {
                PlanName = dto.PlanName,
                Price = dto.Price,
                MaxMoviesPerMonth = dto.MaxMoviesPerMonth,
                MaxChangePerMonth = dto.MaxChangePerMonth,
                Description = dto.Description
            };

            await _membershipPlanRepository.AddAsync(plan);
        }

        // PLAN GÜNCELLEME (bool ile başarı bilgisi dönüyoruz)
        public async Task<bool> UpdatePlan(UpdateMembershipPlanDto dto)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(dto.Id);
            if (plan == null)
                return false;

            plan.PlanName = dto.PlanName;
            plan.Price = dto.Price;
            plan.MaxMoviesPerMonth = dto.MaxMoviesPerMonth;
            plan.MaxChangePerMonth = dto.MaxChangePerMonth;
            plan.Description = dto.Description;

            await _membershipPlanRepository.UpdateAsync(plan);
            return true;
        }

        // PLAN SİLME
        public async Task<bool> DeletePlan(int id)
        {
            var plan = await _membershipPlanRepository.GetByIdAsync(id);
            if (plan == null)
                return false;

            await _membershipPlanRepository.DeleteAsync(plan);
            return true;
        }
    }
}
