// DOSYA YOLU:
// Infrastructure/Repositories/DirectorRepository.cs

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
    public class DirectorRepository : IDirectorRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public DirectorRepository(FilmDukkaniDbContext context)
        {
            _context = context;
        }

        public async Task<Director> GetByIdAsync(int id)
        {
            var director = await _context.Set<Director>().FirstOrDefaultAsync(x => x.ID == id);
            if (director == null)
                throw new InvalidOperationException("Director not found.");

            return director;
        }

        public async Task<IReadOnlyList<Director>> GetAllAsync()
        {
            return await _context.Set<Director>().ToListAsync();
        }

        public async Task<IReadOnlyList<Director>> GetAllAsync(Expression<Func<Director, bool>> predicate)
        {
            return await _context.Set<Director>().Where(predicate).ToListAsync();
        }

        public async Task<Director> AddAsync(Director entity)
        {
            await _context.Set<Director>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Director entity)
        {
            _context.Set<Director>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Director entity)
        {
            _context.Set<Director>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Director>> GetAllAsNoTrackingAsync()
        {
            return await _context.Directors
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Director?> FindByNameAsync(string firstName, string lastName)
        {
            firstName = (firstName ?? "").Trim();
            lastName = (lastName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                return null;

            return await _context.Directors
                .AsNoTracking()
                .FirstOrDefaultAsync(d =>
                    d.FirstName.ToLower() == firstName.ToLower() &&
                    d.LastName.ToLower() == lastName.ToLower());
        }
    }
}
