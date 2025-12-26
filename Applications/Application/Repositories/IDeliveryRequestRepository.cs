using Application.DTOs.DeliveryRequestDTOs;
using Domain.Entities;
using Domain.Enums;

namespace Application.Repositories
{
    public interface IDeliveryRequestRepository : IRepository<DeliveryRequest>
    {
        Task<List<DeliveryRequestListDto>> GetByMemberAsync(int memberId);

        // Liste kilidi: Pending/Prepared/Shipped/Delivered/CancelRequested iken true
        Task<bool> HasActiveRequestForListAsync(int memberId, int listId);

        // Admin ekranı için: iptal talepleri
        Task<List<DeliveryRequestListDto>> GetCancelRequestedAsync();
    }
}
