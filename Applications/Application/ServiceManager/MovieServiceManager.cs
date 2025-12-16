using Application.DTOs.MovieDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MovieServiceManager
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;

        public MovieServiceManager(IMovieRepository movieRepository, ICategoryRepository categoryRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
        }

        // LISTELEME
        public async Task<List<MovieDto>> GetMoviesAsync()
        {
            var movies = await _movieRepository.GetAllAsync();

            return movies
                .Select(m => new MovieDto
                {
                    Id = m.ID,
                    Title = m.Title,
                    OriginalTitle = m.OriginalTitle,
                    Description = m.Description,
                    ReleaseYear = m.ReleaseYear,
                    TechnicalDetails = m.TechnicalDetails,
                    AudioFeatures = m.AudioFeatures,
                    SubtitleLanguages = m.SubtitleLanguages,
                    TrailerUrl = m.TrailerUrl,
                    CoverImageUrl = m.CoverImageUrl,
                    Barcode = m.Barcode,
                    Supplier = m.Supplier,
                    CategoryId = m.CategoryId,
                    CategoryName = m.Category != null ? m.Category.CategoryName : null
                })
                .ToList();
        }

        // EKLEME (Kategori yoksa patlatma -> false)
        public async Task<bool> AddMovie(CreateMovieDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            var movie = new Movie
            {
                Title = dto.Title,
                OriginalTitle = dto.OriginalTitle,
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear,
                TechnicalDetails = dto.TechnicalDetails,
                AudioFeatures = dto.AudioFeatures,
                SubtitleLanguages = dto.SubtitleLanguages,
                TrailerUrl = dto.TrailerUrl,
                CoverImageUrl = dto.CoverImageUrl,
                Barcode = dto.Barcode,
                Supplier = dto.Supplier,
                CategoryId = dto.CategoryId
            };

            await _movieRepository.AddAsync(movie);
            return true;
        }

        // TEK GETIRME (Update için)
        public async Task<UpdateMovieDto?> GetMovie(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null) return null;

            return new UpdateMovieDto
            {
                Id = movie.ID,
                Title = movie.Title,
                OriginalTitle = movie.OriginalTitle,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                TechnicalDetails = movie.TechnicalDetails,
                AudioFeatures = movie.AudioFeatures,
                SubtitleLanguages = movie.SubtitleLanguages,
                TrailerUrl = movie.TrailerUrl,
                CoverImageUrl = movie.CoverImageUrl,
                Barcode = movie.Barcode,
                Supplier = movie.Supplier,
                CategoryId = movie.CategoryId
            };
        }

        // FILM DETAYI
        public async Task<MovieDetailDto?> GetMovieDetailAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return null;

            // Category navigation null gelebileceği için garanti dolduralım
            var category = await _categoryRepository.GetByIdAsync(movie.CategoryId);
            var categoryName = category?.CategoryName ?? string.Empty;

            return new MovieDetailDto
            {
                Id = movie.ID,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                CategoryId = movie.CategoryId,
                CategoryName = categoryName,

                OriginalTitle = movie.OriginalTitle,
                TechnicalDetails = movie.TechnicalDetails,
                AudioFeatures = movie.AudioFeatures,
                SubtitleLanguages = movie.SubtitleLanguages,
                TrailerUrl = movie.TrailerUrl,
                CoverImageUrl = movie.CoverImageUrl,

                Actors = new List<string>(),
                Directors = new List<string>(),
                Awards = new List<MovieAwardInfoDto>()
            };
        }

        // SILME (Controller: Delete(int id) ile uyumlu)
        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return false;

            await _movieRepository.DeleteAsync(movie);
            return true;
        }

        // GUNCELLEME (Kategori yoksa patlatma -> false)
        public async Task<bool> UpdateMovie(UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(dto.Id);
            if (movie == null)
                return false;

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
                return false;

            movie.Title = dto.Title;
            movie.OriginalTitle = dto.OriginalTitle;
            movie.Description = dto.Description;
            movie.ReleaseYear = dto.ReleaseYear;
            movie.TechnicalDetails = dto.TechnicalDetails;
            movie.AudioFeatures = dto.AudioFeatures;
            movie.SubtitleLanguages = dto.SubtitleLanguages;
            movie.TrailerUrl = dto.TrailerUrl;
            movie.CoverImageUrl = dto.CoverImageUrl;
            movie.Barcode = dto.Barcode;
            movie.Supplier = dto.Supplier;
            movie.CategoryId = dto.CategoryId;

            await _movieRepository.UpdateAsync(movie);
            return true;
        }
    }
}
