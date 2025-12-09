using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class AwardRepository : BaseRepository<Award>, IAwardRepository
    {
        public AwardRepository(FilmDukkaniDbContext context) : base(context)
        {
        }
    }
}
