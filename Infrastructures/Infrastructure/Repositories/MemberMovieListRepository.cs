using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class MemberMovieListRepository
        : BaseRepository<MemberMovieList>, IMemberMovieListRepository
    {
        public MemberMovieListRepository(FilmDukkaniDbContext context)
            : base(context)
        {
        }
    }
}
