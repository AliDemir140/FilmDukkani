using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
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

        public async Task<List<Movie>> GetTopRentedMoviesAsync(int take)
        {
            if (take <= 0) take = 10;

            var top = await _context.DeliveryRequestItems
                .AsNoTracking()
                .Join(_context.DeliveryRequests.AsNoTracking(),
                      i => i.DeliveryRequestId,
                      r => r.ID,
                      (i, r) => new { i.MovieId, r.Status })
                .Where(x => x.Status == DeliveryStatus.Delivered || x.Status == DeliveryStatus.Completed)
                .GroupBy(x => x.MovieId)
                .Select(g => new { MovieId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(take)
                .ToListAsync();

            if (top.Count == 0)
                return new List<Movie>();

            var ids = top.Select(x => x.MovieId).ToList();

            var movies = await _context.Movies
                .Include(m => m.MovieCategories)
                .ThenInclude(mc => mc.Category)
                .Where(m => ids.Contains(m.ID))
                .ToListAsync();

            var order = ids.Select((id, index) => new { id, index })
                           .ToDictionary(x => x.id, x => x.index);

            return movies
                .OrderBy(m => order.TryGetValue(m.ID, out var idx) ? idx : int.MaxValue)
                .ToList();
        }
    }
}
