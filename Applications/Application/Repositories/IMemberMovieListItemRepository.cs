using Domain.Entities;

namespace Application.Repositories
{
    public interface IMemberMovieListItemRepository : IRepository<MemberMovieListItem>
    {
        // İleride: Belirli bir listenin tüm item'larını getir gibi ekstra metotlar eklenebilir
    }
}
