using Application.DTOs.DeliveryRequestDTOs;
using Domain.Entities;

namespace Application.Repositories
{
    public interface IDeliveryRequestRepository : IRepository<DeliveryRequest>
    {
        Task<List<DeliveryRequestListDto>> GetByMemberAsync(int memberId);
    }
}
