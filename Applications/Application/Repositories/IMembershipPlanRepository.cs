using Domain.Entities;

namespace Application.Repositories
{
    public interface IMembershipPlanRepository : IRepository<MembershipPlan>
    {
        // Şimdilik ekstra bir metod gerekmiyor.
        // İleride: PlanName'e göre arama, aktif plan listeleme gibi şeyler ekleyebiliriz.
    }
}
