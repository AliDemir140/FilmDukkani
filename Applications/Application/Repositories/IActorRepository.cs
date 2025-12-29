using Domain.Entities;

namespace Application.Repositories
{
    public interface IActorRepository : IRepository<Actor>
    {
        Task<List<Actor>> GetAllAsNoTrackingAsync();
        Task<Actor?> FindByNameAsync(string firstName, string lastName);
    }
}
