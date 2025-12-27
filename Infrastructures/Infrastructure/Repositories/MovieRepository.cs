using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public MovieRepository(FilmDukkaniDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> GetByIdAsync(int id)
        {
            var movie = await _context.Set<Movie>().FirstOrDefaultAsync(x => x.ID == id);
            if (movie == null)
                throw new InvalidOperationException("Movie not found.");

            return movie;
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync()
        {
            return await _context.Set<Movie>().ToListAsync();
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync(Expression<Func<Movie, bool>> predicate)
        {
            return await _context.Set<Movie>().Where(predicate).ToListAsync();
        }

        public async Task<Movie> AddAsync(Movie entity)
        {
            await _context.Set<Movie>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Movie entity)
        {
            _context.Set<Movie>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie entity)
        {
            _context.Set<Movie>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Movie>> GetMoviesWithCategoryAsync()
        {
            return await _context.Movies
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .ToListAsync();
        }

        public async Task<Movie?> GetMovieWithCategoryAsync(int id)
        {
            return await _context.Movies
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task<List<Movie>> GetEditorsChoiceMoviesAsync()
        {
            return await _context.Movies
                .Where(m => m.IsEditorsChoice)
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .ToListAsync();
        }

        public async Task<List<Movie>> GetNewReleaseMoviesAsync()
        {
            return await _context.Movies
                .Where(m => m.IsNewRelease)
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .ToListAsync();
        }
    }
}
