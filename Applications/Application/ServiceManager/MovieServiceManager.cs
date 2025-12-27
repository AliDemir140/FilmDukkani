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

        public async Task<List<MovieDto>> GetMoviesAsync()
        {
            var movies = await _movieRepository.GetMoviesWithCategoryAsync();
            return MapToMovieDtoList(movies);
        }

        public async Task<List<MovieDto>> SearchMoviesAsync(int? categoryId, string? q)
        {
            var movies = await _movieRepository.SearchMoviesAsync(categoryId, q);
            return MapToMovieDtoList(movies);
        }

        public async Task<List<MovieDto>> GetEditorsChoiceAsync()
        {
            var movies = await _movieRepository.GetEditorsChoiceMoviesAsync();
            return MapToMovieDtoList(movies);
        }

        public async Task<List<MovieDto>> GetNewReleasesAsync()
        {
            var movies = await _movieRepository.GetNewReleaseMoviesAsync();
            return MapToMovieDtoList(movies);
        }

        public async Task<bool> AddMovie(CreateMovieDto dto)
        {
            if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
                return false;

            var distinctIds = dto.CategoryIds.Where(x => x > 0).Distinct().ToList();
            if (distinctIds.Count == 0)
                return false;

            foreach (var cid in distinctIds)
            {
                var category = await _categoryRepository.GetByIdAsync(cid);
                if (category == null)
                    return false;
            }

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
                IsEditorsChoice = dto.IsEditorsChoice,
                IsNewRelease = dto.IsNewRelease,
                MovieCategories = distinctIds.Select(cid => new MovieCategory
                {
                    CategoryId = cid
                }).ToList()
            };

            await _movieRepository.AddAsync(movie);
            return true;
        }

        public async Task<UpdateMovieDto?> GetMovie(int id)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(id);
            if (movie == null)
                return null;

            var categoryIds = movie.MovieCategories?
                .Select(x => x.CategoryId)
                .Distinct()
                .ToList() ?? new List<int>();

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
                IsEditorsChoice = movie.IsEditorsChoice,
                IsNewRelease = movie.IsNewRelease,
                CategoryIds = categoryIds
            };
        }

        public async Task<MovieDetailDto?> GetMovieDetailAsync(int id)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(id);
            if (movie == null)
                return null;

            var categoryIds = movie.MovieCategories?
                .Select(x => x.CategoryId)
                .Distinct()
                .ToList() ?? new List<int>();

            var categoryNames = movie.MovieCategories?
                .Where(x => x.Category != null && !string.IsNullOrWhiteSpace(x.Category.CategoryName))
                .Select(x => x.Category.CategoryName)
                .Distinct()
                .ToList() ?? new List<string>();

            return new MovieDetailDto
            {
                Id = movie.ID,
                Title = movie.Title,
                Description = movie.Description ?? string.Empty,
                ReleaseYear = movie.ReleaseYear,
                IsEditorsChoice = movie.IsEditorsChoice,
                IsNewRelease = movie.IsNewRelease,
                CategoryIds = categoryIds,
                CategoryNames = categoryNames,
                CategoryId = categoryIds.FirstOrDefault(),
                CategoryName = categoryNames.FirstOrDefault(),
                OriginalTitle = movie.OriginalTitle ?? string.Empty,
                TechnicalDetails = movie.TechnicalDetails ?? string.Empty,
                AudioFeatures = movie.AudioFeatures ?? string.Empty,
                SubtitleLanguages = movie.SubtitleLanguages ?? string.Empty,
                TrailerUrl = movie.TrailerUrl ?? string.Empty,
                CoverImageUrl = movie.CoverImageUrl ?? string.Empty,
                Actors = new List<string>(),
                Directors = new List<string>(),
                Awards = new List<MovieAwardInfoDto>()
            };
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(id);
            if (movie == null)
                return false;

            await _movieRepository.DeleteAsync(movie);
            return true;
        }

        public async Task<bool> UpdateMovie(UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(dto.Id);
            if (movie == null)
                return false;

            if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
                return false;

            var distinctIds = dto.CategoryIds.Where(x => x > 0).Distinct().ToList();
            if (distinctIds.Count == 0)
                return false;

            foreach (var cid in distinctIds)
            {
                var category = await _categoryRepository.GetByIdAsync(cid);
                if (category == null)
                    return false;
            }

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
            movie.IsEditorsChoice = dto.IsEditorsChoice;
            movie.IsNewRelease = dto.IsNewRelease;

            movie.MovieCategories ??= new List<MovieCategory>();

            var existingIds = movie.MovieCategories.Select(x => x.CategoryId).Distinct().ToList();

            var toRemove = movie.MovieCategories.Where(x => !distinctIds.Contains(x.CategoryId)).ToList();
            foreach (var rm in toRemove)
                movie.MovieCategories.Remove(rm);

            var toAdd = distinctIds.Where(x => !existingIds.Contains(x)).ToList();
            foreach (var cid in toAdd)
            {
                movie.MovieCategories.Add(new MovieCategory
                {
                    CategoryId = cid,
                    MovieId = movie.ID
                });
            }

            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        private static List<MovieDto> MapToMovieDtoList(List<Movie> movies)
        {
            return movies
                .Select(m =>
                {
                    var categoryIds = m.MovieCategories?
                        .Select(x => x.CategoryId)
                        .Distinct()
                        .ToList() ?? new List<int>();

                    var categoryNames = m.MovieCategories?
                        .Select(x => x.Category != null ? x.Category.CategoryName : null)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .Select(x => x!)
                        .Distinct()
                        .ToList() ?? new List<string>();

                    return new MovieDto
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
                        IsEditorsChoice = m.IsEditorsChoice,
                        IsNewRelease = m.IsNewRelease,
                        CategoryIds = categoryIds,
                        CategoryNames = categoryNames,
                        CategoryId = categoryIds.FirstOrDefault(),
                        CategoryName = categoryNames.FirstOrDefault()
                    };
                })
                .ToList();
        }
    }
}
