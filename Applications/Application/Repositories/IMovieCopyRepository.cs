using Domain.Entities;

namespace Application.Repositories
{
    public interface IMovieCopyRepository : IRepository<MovieCopy>
    {
        Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null);
    }
}
