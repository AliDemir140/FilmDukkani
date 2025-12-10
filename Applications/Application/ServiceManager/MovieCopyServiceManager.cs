using Application.DTOs.MovieCopyDTOs;
using Application.Repositories;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // Tüm kopyaları listele
        public async Task<List<MovieCopyDto>> GetMovieCopiesAsync()
        {
            var copies = await _movieCopyRepository.GetAllAsync();

            var movieIds = copies.Select(c => c.MovieId).Distinct().ToList();
            var shelfIds = copies.Where(c => c.ShelfId.HasValue)
                                 .Select(c => c.ShelfId.Value)
                                 .Distinct()
                                 .ToList();

            var movies = await _movieRepository.GetAllAsync(m => movieIds.Contains(m.ID));
            var shelves = await _shelfRepository.GetAllAsync(s => shelfIds.Contains(s.ID));

            return copies
                .Select(c =>
                {
                    var movie = movies.FirstOrDefault(m => m.ID == c.MovieId);
                    var shelf = c.ShelfId.HasValue
                        ? shelves.FirstOrDefault(s => s.ID == c.ShelfId.Value)
                        : null;

                    return new MovieCopyDto
                    {
                        Id = c.ID,
                        MovieId = c.MovieId,
                        MovieTitle = movie?.Title ?? string.Empty,
                        Barcode = c.Barcode,
                        ShelfId = c.ShelfId,
                        ShelfName = shelf?.Name,
                        IsAvailable = c.IsAvailable,
                        IsDamaged = c.IsDamaged,
                        CreatedDate = c.CreatedDate,
                        ModifiedDate = c.ModifiedDate
                    };
                })
                .ToList();
        }

        // Tek kopyayı getir (update formu vb.)
        public async Task<UpdateMovieCopyDto?> GetMovieCopyAsync(int id)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(id);
            if (copy == null)
                return null;

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

        // Yeni kopya ekle
        public async Task AddMovieCopyAsync(CreateMovieCopyDto dto)
        {
            var copy = new MovieCopy
            {
                MovieId = dto.MovieId,
                Barcode = dto.Barcode,
                ShelfId = dto.ShelfId,
                IsAvailable = true,
                IsDamaged = false
            };

            await _movieCopyRepository.AddAsync(copy);
        }

        // Kopyayı güncelle
        public async Task<bool> UpdateMovieCopyAsync(UpdateMovieCopyDto dto)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(dto.Id);
            if (copy == null)
                return false;

            copy.MovieId = dto.MovieId;
            copy.Barcode = dto.Barcode;
            copy.ShelfId = dto.ShelfId;
            copy.IsAvailable = dto.IsAvailable;
            copy.IsDamaged = dto.IsDamaged;

            await _movieCopyRepository.UpdateAsync(copy);
            return true;
        }

        // Kopyayı sil
        public async Task<bool> DeleteMovieCopyAsync(int id)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(id);
            if (copy == null)
                return false;

            await _movieCopyRepository.DeleteAsync(copy);
            return true;
        }
    }
}
