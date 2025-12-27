using Domain.Entities;

namespace Application.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<List<Movie>> GetMoviesWithCategoryAsync();
        Task<Movie?> GetMovieWithCategoryAsync(int id);

        Task<List<Movie>> GetEditorsChoiceMoviesAsync();
        Task<List<Movie>> GetNewReleaseMoviesAsync();

        Task<List<Movie>> SearchMoviesAsync(int? categoryId, string? q);
        Task<List<Movie>> GetTopRentedMoviesAsync(int take);

        Task<List<Movie>> GetAwardWinnersAsync(int take);
    }
}
