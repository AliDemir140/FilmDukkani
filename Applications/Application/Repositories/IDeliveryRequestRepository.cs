using Application.DTOs.DeliveryRequestDTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IDeliveryRequestRepository : IRepository<DeliveryRequest>
    {
        Task<List<DeliveryRequestListDto>> GetByMemberAsync(int memberId);

        // Aynı liste için aktif sipariş var mı? (Pending/Prepared/Shipped/Delivered)
        Task<bool> HasActiveRequestForListAsync(int memberId, int listId);
    }
}
