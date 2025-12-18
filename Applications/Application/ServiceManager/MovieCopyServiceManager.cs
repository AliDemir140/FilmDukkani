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

        public async Task<bool> AddMovieCopyAsync(CreateMovieCopyDto dto)
        {
            // Film var mı?
            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null) return false;

            // Barkod zorunlu (dto Required var ama ekstra güvenlik)
            if (string.IsNullOrWhiteSpace(dto.Barcode)) return false;

            var copy = new MovieCopy
            {
                MovieId = dto.MovieId,
                Barcode = dto.Barcode.Trim(),
                ShelfId = dto.ShelfId,
                IsAvailable = dto.IsAvailable,
                IsDamaged = dto.IsDamaged
            };

            await _movieCopyRepository.AddAsync(copy);
            return true;
        }

        public async Task<bool> UpdateMovieCopyAsync(UpdateMovieCopyDto dto)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(dto.Id);
            if (copy == null) return false;

            var movie = await _movieRepository.GetByIdAsync(dto.MovieId);
            if (movie == null) return false;

            if (string.IsNullOrWhiteSpace(dto.Barcode)) return false;

            copy.MovieId = dto.MovieId;
            copy.Barcode = dto.Barcode.Trim();
            copy.ShelfId = dto.ShelfId;
            copy.IsAvailable = dto.IsAvailable;
            copy.IsDamaged = dto.IsDamaged;

            await _movieCopyRepository.UpdateAsync(copy);
            return true;
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
