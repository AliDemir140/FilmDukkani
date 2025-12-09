using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class MovieCopyRepository : BaseRepository<MovieCopy>, IMovieCopyRepository
    {
        public MovieCopyRepository(FilmDukkaniDbContext context) : base(context)
        {
        }
    }
}
