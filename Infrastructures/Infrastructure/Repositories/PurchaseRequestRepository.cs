using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class PurchaseRequestRepository : IPurchaseRequestRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public PurchaseRequestRepository(FilmDukkaniDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseRequest> GetByIdAsync(int id)
        {
            var entity = await _context.Set<PurchaseRequest>().FirstOrDefaultAsync(x => x.ID == id);
            if (entity == null)
                throw new InvalidOperationException("PurchaseRequest not found.");

            return entity;
        }

        public async Task<IReadOnlyList<PurchaseRequest>> GetAllAsync()
        {
            return await _context.Set<PurchaseRequest>().ToListAsync();
        }

        public async Task<IReadOnlyList<PurchaseRequest>> GetAllAsync(Expression<Func<PurchaseRequest, bool>> predicate)
        {
            return await _context.Set<PurchaseRequest>().Where(predicate).ToListAsync();
        }

        public async Task<PurchaseRequest> AddAsync(PurchaseRequest entity)
        {
            await _context.Set<PurchaseRequest>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(PurchaseRequest entity)
        {
            _context.Set<PurchaseRequest>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(PurchaseRequest entity)
        {
            _context.Set<PurchaseRequest>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PurchaseRequest>> GetAllWithMovieAsync()
        {
            return await _context.PurchaseRequests
                .AsNoTracking()
                .Include(x => x.Movie)
                .Include(x => x.Member)
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        public async Task<PurchaseRequest?> GetByIdWithMovieAsync(int id)
        {
            return await _context.PurchaseRequests
                .Include(x => x.Movie)
                .Include(x => x.Member)
                .FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<List<PurchaseRequest>> GetByMemberAsync(int memberId)
        {
            return await _context.PurchaseRequests
                .AsNoTracking()
                .Include(x => x.Movie)
                .Where(x => x.MemberId == memberId)
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }

        public async Task<List<PurchaseRequest>> GetPendingAsync()
        {
            return await _context.PurchaseRequests
                .AsNoTracking()
                .Include(x => x.Movie)
                .Include(x => x.Member)
                .Where(x => x.Status == PurchaseRequestStatus.Pending)
                .OrderByDescending(x => x.ID)
                .ToListAsync();
        }
    }
}
