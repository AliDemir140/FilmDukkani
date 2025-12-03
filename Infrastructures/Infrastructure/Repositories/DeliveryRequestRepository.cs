using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class DeliveryRequestRepository : BaseRepository<DeliveryRequest>, IDeliveryRequestRepository
    {
        public DeliveryRequestRepository(FilmDukkaniDbContext context) : base(context)
        {
        }
    }
}
