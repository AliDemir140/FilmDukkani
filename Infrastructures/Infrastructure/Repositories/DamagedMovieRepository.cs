using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class DamagedMovieRepository : BaseRepository<DamagedMovie>, IDamagedMovieRepository
    {
        public DamagedMovieRepository(FilmDukkaniDbContext context) : base(context)
        {
        }
    }
}
