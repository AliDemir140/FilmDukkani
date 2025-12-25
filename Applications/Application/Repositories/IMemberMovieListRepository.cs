using Domain.Entities;

namespace Application.Repositories
{
    public interface IMemberMovieListRepository : IRepository<MemberMovieList>
    {
        Task<bool> ExistsByNameAsync(int memberId, string name);
    }
}
