using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IPurchaseRequestRepository : IRepository<PurchaseRequest>
    {
        Task<List<PurchaseRequest>> GetAllWithMovieAsync();
        Task<PurchaseRequest?> GetByIdWithMovieAsync(int id);

        Task<List<PurchaseRequest>> GetByMemberAsync(int memberId);
        Task<List<PurchaseRequest>> GetPendingAsync();
    }
}
