using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        public MovieRepository(FilmDukkaniDbContext context) : base(context)
        {
        }

        public async Task<List<Movie>> GetMoviesWithCategoryAsync()
        {
            return await _context.Movies
                .Include(m => m.Category)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieWithCategoryAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
        }
    }
}
