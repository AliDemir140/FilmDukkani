// DOSYA YOLU:
// Infrastructure/Repositories/ActorRepository.cs

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
    public class ActorRepository : IActorRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public ActorRepository(FilmDukkaniDbContext context)
        {
            _context = context;
        }

        public async Task<Actor> GetByIdAsync(int id)
        {
            var actor = await _context.Set<Actor>().FirstOrDefaultAsync(x => x.ID == id);
            if (actor == null)
                throw new InvalidOperationException("Actor not found.");

            return actor;
        }

        public async Task<IReadOnlyList<Actor>> GetAllAsync()
        {
            return await _context.Set<Actor>().ToListAsync();
        }

        public async Task<IReadOnlyList<Actor>> GetAllAsync(Expression<Func<Actor, bool>> predicate)
        {
            return await _context.Set<Actor>().Where(predicate).ToListAsync();
        }

        public async Task<Actor> AddAsync(Actor entity)
        {
            await _context.Set<Actor>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Actor entity)
        {
            _context.Set<Actor>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Actor entity)
        {
            _context.Set<Actor>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Actor>> GetAllAsNoTrackingAsync()
        {
            return await _context.Actors
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Actor?> FindByNameAsync(string firstName, string lastName)
        {
            firstName = (firstName ?? "").Trim();
            lastName = (lastName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(firstName))
                return null;

            return await _context.Actors
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.FirstName.ToLower() == firstName.ToLower() &&
                    a.LastName.ToLower() == lastName.ToLower());
        }
    }
}
