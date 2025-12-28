using Application.DTOs.PurchaseRequestDTOs;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;

namespace Application.ServiceManager
{
    public class PurchaseRequestServiceManager
    {
        private readonly IPurchaseRequestRepository _purchaseRequestRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemberRepository _memberRepository;

        public PurchaseRequestServiceManager(
            IPurchaseRequestRepository purchaseRequestRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository)
        {
            _purchaseRequestRepository = purchaseRequestRepository;
            _movieRepository = movieRepository;
            _memberRepository = memberRepository;
        }

        public async Task<List<PurchaseRequestDto>> GetAllAsync()
        {
            var list = await _purchaseRequestRepository.GetAllWithMovieAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<List<PurchaseRequestDto>> GetPendingAsync()
        {
            var list = await _purchaseRequestRepository.GetPendingAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<List<PurchaseRequestDto>> GetByMemberAsync(int memberId)
        {
            if (memberId <= 0)
                return new List<PurchaseRequestDto>();

            var list = await _purchaseRequestRepository.GetByMemberAsync(memberId);
            return list.Select(MapToDto).ToList();
        }

        public async Task<bool> CreateAsync(CreatePurchaseRequestDto dto)
        {
            if (dto.MemberId <= 0 || dto.MovieId <= 0 || dto.Quantity <= 0)
                return false;

            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null)
                return false;

            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null)
                return false;

            var entity = new PurchaseRequest
            {
                MemberId = dto.MemberId,
                MovieId = dto.MovieId,
                Quantity = dto.Quantity,
                Note = dto.Note,
                Status = PurchaseRequestStatus.Pending
            };

            await _purchaseRequestRepository.AddAsync(entity);
            return true;
        }

        public async Task<bool> DecideAsync(DecidePurchaseRequestDto dto)
        {
            if (dto.RequestId <= 0)
                return false;

            var entity = await _purchaseRequestRepository.GetByIdAsync(dto.RequestId);
            if (entity.Status != PurchaseRequestStatus.Pending)
                return false;

            entity.Status = dto.Approved ? PurchaseRequestStatus.Approved : PurchaseRequestStatus.Rejected;
            entity.DecisionAt = DateTime.Now;
            entity.DecisionNote = dto.AdminNote;

            await _purchaseRequestRepository.UpdateAsync(entity);
            return true;
        }

        private static PurchaseRequestDto MapToDto(PurchaseRequest x)
        {
            return new PurchaseRequestDto
            {
                Id = x.ID,
                MovieId = x.MovieId,
                MovieTitle = x.Movie?.Title ?? "",
                Quantity = x.Quantity,
                Note = x.Note,
                Status = x.Status,
                CreatedAt = x.CreatedDate,
                DecisionAt = x.DecisionAt,
                DecisionNote = x.DecisionNote
            };
        }
    }
}
