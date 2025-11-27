using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class MembershipPlanRepository : BaseRepository<MembershipPlan>, IMembershipPlanRepository
    {
        public MembershipPlanRepository(FilmDukkaniDbContext context)
            : base(context)
        {
        }
    }
}
