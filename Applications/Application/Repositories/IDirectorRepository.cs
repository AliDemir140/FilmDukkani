using Domain.Entities;

namespace Application.Repositories
{
    public interface IDirectorRepository : IRepository<Director>
    {
        Task<List<Director>> GetAllAsNoTrackingAsync();
        Task<Director?> FindByNameAsync(string firstName, string lastName);
    }
}
