using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories
{
    public class MemberMovieListItemRepository
        : BaseRepository<MemberMovieListItem>, IMemberMovieListItemRepository
    {
        public MemberMovieListItemRepository(FilmDukkaniDbContext context)
            : base(context)
        {
        }
    }
}
