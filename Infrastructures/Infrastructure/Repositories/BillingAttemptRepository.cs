using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BillingAttemptRepository : BaseRepository<BillingAttempt>, IBillingAttemptRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public BillingAttemptRepository(FilmDukkaniDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsForPeriodAsync(int memberId, string period)
        {
            return await _context.BillingAttempts
                .AsNoTracking()
                .AnyAsync(x => x.MemberId == memberId && x.Period == period);
        }
    }
}
