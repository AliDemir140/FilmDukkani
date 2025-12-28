using Application.DTOs.MovieCopyDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MovieCopyServiceManager
    {
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IShelfRepository _shelfRepository;

        public MovieCopyServiceManager(
            IMovieCopyRepository movieCopyRepository,
            IMovieRepository movieRepository,
            IShelfRepository shelfRepository)
        {
            _movieCopyRepository = movieCopyRepository;
            _movieRepository = movieRepository;
            _shelfRepository = shelfRepository;
        }

        private static string BuildStandardBarcode(int movieCopyId)
        {
            return "FDK-MC-" + movieCopyId.ToString("D8");
        }

        private static string BuildTempBarcode()
        {
            return "TMP-" + Guid.NewGuid().ToString("N");
        }

        public async Task<List<MovieCopyDto>> GetMovieCopiesAsync()
        {
            var copies = await _movieCopyRepository.GetAllAsync();

            var movieIds = copies.Select(c => c.MovieId).Distinct().ToList();
            var shelfIds = copies.Where(c => c.ShelfId.HasValue).Select(c => c.ShelfId!.Value).Distinct().ToList();

            var movies = movieIds.Count == 0
                ? new List<Movie>()
                : await _movieRepository.GetAllAsync(m => movieIds.Contains(m.ID));

            var shelves = shelfIds.Count == 0
                ? new List<Shelf>()
                : await _shelfRepository.GetAllAsync(s => shelfIds.Contains(s.ID));

            return copies.Select(c =>
            {
                var movie = movies.FirstOrDefault(m => m.ID == c.MovieId);
                var shelf = c.ShelfId.HasValue ? shelves.FirstOrDefault(s => s.ID == c.ShelfId.Value) : null;

                return new MovieCopyDto
                {
                    Id = c.ID,
                    MovieId = c.MovieId,
                    MovieTitle = movie?.Title ?? "",
                    Barcode = c.Barcode,
                    ShelfId = c.ShelfId,
                    ShelfName = shelf?.Name,
                    IsAvailable = c.IsAvailable,
                    IsDamaged = c.IsDamaged,
                    CreatedDate = c.CreatedDate,
                    ModifiedDate = c.ModifiedDate
                };
            }).ToList();
        }

        public async Task<UpdateMovieCopyDto?> GetMovieCopyAsync(int id)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(id);
            if (copy == null) return null;

            return new UpdateMovieCopyDto
            {
                Id = copy.ID,
                MovieId = copy.MovieId,
                Barcode = copy.Barcode,
                ShelfId = copy.ShelfId,
                IsAvailable = copy.IsAvailable,
                IsDamaged = copy.IsDamaged
            };
        }

        public async Task<(bool Ok, string Error)> AddMovieCopyAsync(CreateMovieCopyDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null) return (false, "Film bulunamadı.");

            var manualBarcode = (dto.Barcode ?? "").Trim();

            if (!string.IsNullOrWhiteSpace(manualBarcode))
            {
                if (await _movieCopyRepository.BarcodeExistsAsync(manualBarcode))
                    return (false, "Bu barkod zaten kullanılıyor.");

                var copyManual = new MovieCopy
                {
                    MovieId = dto.MovieId,
                    Barcode = manualBarcode,
                    ShelfId = dto.ShelfId,
                    IsAvailable = dto.IsAvailable,
                    IsDamaged = dto.IsDamaged
                };

                await _movieCopyRepository.AddAsync(copyManual);
                return (true, "");
            }

            var temp = BuildTempBarcode();
            while (await _movieCopyRepository.BarcodeExistsAsync(temp))
            {
                temp = BuildTempBarcode();
            }

            var copy = new MovieCopy
            {
                MovieId = dto.MovieId,
                Barcode = temp,
                ShelfId = dto.ShelfId,
                IsAvailable = dto.IsAvailable,
                IsDamaged = dto.IsDamaged
            };

            await _movieCopyRepository.AddAsync(copy);

            var standard = BuildStandardBarcode(copy.ID);

            if (await _movieCopyRepository.BarcodeExistsAsync(standard, excludeId: copy.ID))
            {
                return (false, "Otomatik barkod üretilemedi. Barkod çakışması var.");
            }

            copy.Barcode = standard;
            await _movieCopyRepository.UpdateAsync(copy);

            return (true, "");
        }

        public async Task<(bool Ok, string Error)> UpdateMovieCopyAsync(UpdateMovieCopyDto dto)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(dto.Id);
            if (copy == null) return (false, "Kopya bulunamadı.");

            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null) return (false, "Film bulunamadı.");

            var barcode = (dto.Barcode ?? "").Trim();
            if (string.IsNullOrWhiteSpace(barcode))
                return (false, "Barkod boş olamaz.");

            if (await _movieCopyRepository.BarcodeExistsAsync(barcode, excludeId: dto.Id))
                return (false, "Bu barkod zaten kullanılıyor.");

            copy.MovieId = dto.MovieId;
            copy.Barcode = barcode;
            copy.ShelfId = dto.ShelfId;
            copy.IsAvailable = dto.IsAvailable;
            copy.IsDamaged = dto.IsDamaged;

            await _movieCopyRepository.UpdateAsync(copy);
            return (true, "");
        }

        public async Task<bool> DeleteMovieCopyAsync(int id)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(id);
            if (copy == null) return false;

            await _movieCopyRepository.DeleteAsync(copy);
            return true;
        }
    }
}
