using Domain.Entities;

namespace Application.Repositories
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<List<Movie>> GetMoviesWithCategoryAsync();
        Task<Movie?> GetMovieWithCategoryAsync(int id);
    }
}
