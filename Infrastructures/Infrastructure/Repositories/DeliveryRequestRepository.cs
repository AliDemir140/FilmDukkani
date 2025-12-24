using Application.DTOs.DeliveryRequestDTOs;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class DeliveryRequestRepository : BaseRepository<DeliveryRequest>, IDeliveryRequestRepository
    {
        public DeliveryRequestRepository(FilmDukkaniDbContext context) : base(context)
        {
        }

        public async Task<List<DeliveryRequestListDto>> GetByMemberAsync(int memberId)
        {
            return await _context.DeliveryRequests
                .AsNoTracking()
                .Where(r => r.MemberId == memberId)
                .OrderByDescending(r => r.ID)
                .Join(_context.MemberMovieLists.AsNoTracking(),
                      r => r.MemberMovieListId,
                      l => l.ID,
                      (r, l) => new DeliveryRequestListDto
                      {
                          Id = r.ID,
                          ListName = l.Name ?? "-",
                          DeliveryDate = r.DeliveryDate,
                          Status = r.Status
                      })
                .ToListAsync();
        }
    }
}
