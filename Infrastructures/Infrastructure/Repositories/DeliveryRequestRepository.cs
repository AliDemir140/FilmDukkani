using Application.DTOs.DeliveryRequestDTOs;
using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace Infrastructure.Repositories
{
    public class DeliveryRequestRepository : Repository<DeliveryRequest>, IDeliveryRequestRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public DeliveryRequestRepository(FilmDukkaniDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DeliveryRequestListDto>> GetByMemberAsync(int memberId)
        {
            return await _context.DeliveryRequests
                .Where(x => x.MemberId == memberId)
                .OrderByDescending(x => x.ID)
                .Select(x => new DeliveryRequestListDto
                {
                    Id = x.ID,
                    MemberId = x.MemberId,
                    MemberMovieListId = x.MemberMovieListId,
                    RequestedDate = x.RequestedDate,
                    DeliveryDate = x.DeliveryDate,
                    Status = x.Status
                })
                .ToListAsync();
        }
    }
}
