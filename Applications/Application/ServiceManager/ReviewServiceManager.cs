using Application.DTOs.ReviewDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class ReviewServiceManager
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IMemberRepository _memberRepository;

        public ReviewServiceManager(
            IReviewRepository reviewRepository,
            IMovieRepository movieRepository,
            IMemberRepository memberRepository)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
            _memberRepository = memberRepository;
        }

        public async Task<int> AddOrUpdateAsync(CreateReviewDto dto)
        {
            if (dto.MovieId <= 0 || dto.MemberId <= 0) return 0;

            var movie = await _movieRepository.GetMovieWithCategoryAsync(dto.MovieId);
            if (movie == null) return -1;

            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null) return -2;

            var existing = await _reviewRepository.GetByMemberAndMovieAsync(dto.MemberId, dto.MovieId);

            if (existing == null)
            {
                var review = new Review
                {
                    MovieId = dto.MovieId,
                    MemberId = dto.MemberId,
                    Rating = dto.Rating,
                    Comment = (dto.Comment ?? "").Trim(),
                    CreatedAt = DateTime.Now
                };

                await _reviewRepository.AddAsync(review);
                return review.ID;
            }

            existing.Rating = dto.Rating;
            existing.Comment = (dto.Comment ?? "").Trim();
            existing.CreatedAt = DateTime.Now;

            await _reviewRepository.UpdateAsync(existing);
            return existing.ID;
        }

        public async Task<bool> DeleteAsync(int reviewId, int memberId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null) return false;

            if (review.MemberId != memberId) return false;

            await _reviewRepository.DeleteAsync(review);
            return true;
        }

        public async Task<List<ReviewDto>> GetMovieReviewsAsync(int movieId, int? currentMemberId)
        {
            var list = await _reviewRepository.GetByMovieAsync(movieId);

            return list.Select(x => new ReviewDto
            {
                Id = x.ID,
                MovieId = x.MovieId,
                MemberId = x.MemberId,
                MemberFullName = x.Member != null ? (x.Member.FirstName + " " + x.Member.LastName) : "",
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.CreatedAt,
                IsOwner = currentMemberId.HasValue && x.MemberId == currentMemberId.Value
            }).ToList();
        }

        public async Task<(double avg, int count)> GetMovieRatingSummaryAsync(int movieId)
        {
            return await _reviewRepository.GetMovieRatingSummaryAsync(movieId);
        }
    }
}
