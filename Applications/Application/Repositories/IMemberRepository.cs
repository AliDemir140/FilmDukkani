using Domain.Entities;

namespace Application.Repositories
{
    public interface IMemberRepository : IRepository<Member>
    {
        // İleride: Email ile arama, login için find vs. eklenebilir.
    }
}
