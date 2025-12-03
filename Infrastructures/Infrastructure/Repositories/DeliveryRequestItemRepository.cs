using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class DeliveryRequestItemRepository : BaseRepository<DeliveryRequestItem>, IDeliveryRequestItemRepository
    {
        public DeliveryRequestItemRepository(FilmDukkaniDbContext context) : base(context)
        {
        }
    }
}
