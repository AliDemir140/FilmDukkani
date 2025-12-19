using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public MovieRepository(FilmDukkaniDbContext context) : base(context)
        {
            _context = context;
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
