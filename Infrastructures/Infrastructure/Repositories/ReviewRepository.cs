using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(FilmDukkaniDbContext context) : base(context)
        {
        }

        public async Task<List<Review>> GetByMovieAsync(int movieId)
        {
            return await _context.Set<Review>()
                .AsNoTracking()
                .Include(x => x.Member)
                .Where(x => x.MovieId == movieId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<(double avg, int count)> GetMovieRatingSummaryAsync(int movieId)
        {
            var query = _context.Set<Review>()
                .AsNoTracking()
                .Where(x => x.MovieId == movieId);

            var count = await query.CountAsync();
            if (count == 0) return (0, 0);

            var avg = await query.AverageAsync(x => (double)x.Rating);
            return (avg, count);
        }

        public async Task<List<int>> GetTopRatedMovieIdsAsync(int take)
        {
            if (take <= 0) take = 10;

            return await _context.Set<Review>()
                .AsNoTracking()
                .GroupBy(x => x.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Avg = g.Average(x => (double)x.Rating),
                    Count = g.Count()
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Avg)
                .ThenByDescending(x => x.Count)
                .Take(take)
                .Select(x => x.MovieId)
                .ToListAsync();
        }

        public async Task<List<int>> GetMostReviewedMovieIdsAsync(int take)
        {
            if (take <= 0) take = 10;

            return await _context.Set<Review>()
                .AsNoTracking()
                .GroupBy(x => x.MovieId)
                .Select(g => new
                {
                    MovieId = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .Select(x => x.MovieId)
                .ToListAsync();
        }

        public async Task<Review?> GetByMemberAndMovieAsync(int memberId, int movieId)
        {
            return await _context.Set<Review>()
                .FirstOrDefaultAsync(x => x.MemberId == memberId && x.MovieId == movieId);
        }
    }
}
