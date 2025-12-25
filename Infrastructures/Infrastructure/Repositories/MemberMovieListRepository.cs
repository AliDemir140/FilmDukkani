using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MemberMovieListRepository
        : BaseRepository<MemberMovieList>, IMemberMovieListRepository
    {
        public MemberMovieListRepository(FilmDukkaniDbContext context)
            : base(context)
        {
        }

        public async Task<bool> ExistsByNameAsync(int memberId, string name)
        {
            name = (name ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            var normalized = name.ToLower();

            return await _context.MemberMovieLists
                .AsNoTracking()
                .AnyAsync(x => x.MemberId == memberId && x.Name.ToLower() == normalized);
        }
    }
}
