using Domain.Entities;

namespace Application.Repositories
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<List<Review>> GetByMovieAsync(int movieId);
        Task<(double avg, int count)> GetMovieRatingSummaryAsync(int movieId);

        Task<List<int>> GetTopRatedMovieIdsAsync(int take);
        Task<List<int>> GetMostReviewedMovieIdsAsync(int take);

        Task<Review?> GetByMemberAndMovieAsync(int memberId, int movieId);
    }
}
