using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IBillingAttemptRepository : IRepository<BillingAttempt>
    {
        Task<bool> ExistsForPeriodAsync(int memberId, string period);
    }
}
