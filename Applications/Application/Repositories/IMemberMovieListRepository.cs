using Domain.Entities;

namespace Application.Repositories
{
    public interface IMemberMovieListRepository : IRepository<MemberMovieList>
    {
        // İleride: Member'a göre aktif liste getir vs. eklenebilir
    }
}
