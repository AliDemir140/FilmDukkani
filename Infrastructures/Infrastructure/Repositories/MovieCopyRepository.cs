using Application.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MovieCopyRepository : BaseRepository<MovieCopy>, IMovieCopyRepository
    {
        private readonly FilmDukkaniDbContext _context;

        public MovieCopyRepository(FilmDukkaniDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null)
        {
            barcode = (barcode ?? "").Trim();
            if (string.IsNullOrWhiteSpace(barcode))
                return false;

            var q = _context.MovieCopies.AsQueryable();

            if (excludeId.HasValue)
                q = q.Where(x => x.ID != excludeId.Value);

            return await q.AnyAsync(x => x.Barcode == barcode);
        }
    }
}
