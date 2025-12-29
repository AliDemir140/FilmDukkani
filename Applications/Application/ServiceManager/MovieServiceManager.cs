using Application.DTOs.MovieDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MovieServiceManager
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IActorRepository _actorRepository;
        private readonly IDirectorRepository _directorRepository;

        public MovieServiceManager(
            IMovieRepository movieRepository,
            ICategoryRepository categoryRepository,
            IReviewRepository reviewRepository,
            IActorRepository actorRepository,
            IDirectorRepository directorRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
            _actorRepository = actorRepository;
            _directorRepository = directorRepository;
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

        public async Task<List<MovieDto>> GetTopRentedAsync(int take = 10)
        {
            var movies = await _movieRepository.GetTopRentedMoviesAsync(take);
            return MapToMovieDtoList(movies);
        }

        public async Task<List<MovieDto>> GetAwardWinnersAsync(int take = 10)
        {
            var movies = await _movieRepository.GetAwardWinnersAsync(take);
            return MapToMovieDtoList(movies);
        }

        public async Task<bool> AddMovie(CreateMovieDto dto)
        {
            if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
                return false;

            var distinctCategoryIds = dto.CategoryIds.Where(x => x > 0).Distinct().ToList();
            if (distinctCategoryIds.Count == 0)
                return false;

            foreach (var cid in distinctCategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(cid);
                if (category == null)
                    return false;
            }

            var actorIds = (dto.ActorIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();
            var directorIds = (dto.DirectorIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            var createdActorIds = await EnsureActorsAsync(dto.NewActors);
            var createdDirectorIds = await EnsureDirectorsAsync(dto.NewDirectors);

            actorIds = actorIds.Concat(createdActorIds).Distinct().ToList();
            directorIds = directorIds.Concat(createdDirectorIds).Distinct().ToList();

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
                IsAwardWinner = dto.IsAwardWinner,

                MovieCategories = distinctCategoryIds.Select(cid => new MovieCategory
                {
                    CategoryId = cid
                }).ToList(),

                MovieActors = actorIds.Select(aid => new MovieActor
                {
                    ActorId = aid
                }).ToList(),

                MovieDirectors = directorIds.Select(did => new MovieDirector
                {
                    DirectorId = did
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

            var actorIds = movie.MovieActors?
                .Select(x => x.ActorId)
                .Distinct()
                .ToList() ?? new List<int>();

            var directorIds = movie.MovieDirectors?
                .Select(x => x.DirectorId)
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
                IsAwardWinner = movie.IsAwardWinner,
                CategoryIds = categoryIds,
                ActorIds = actorIds,
                DirectorIds = directorIds
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

            var actorNames = movie.MovieActors?
                .Where(x => x.Actor != null)
                .Select(x => $"{x.Actor.FirstName} {x.Actor.LastName}".Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            var directorNames = movie.MovieDirectors?
                .Where(x => x.Director != null)
                .Select(x => $"{x.Director.FirstName} {x.Director.LastName}".Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .OrderBy(x => x)
                .ToList() ?? new List<string>();

            var (avg, count) = await _reviewRepository.GetMovieRatingSummaryAsync(movie.ID);

            return new MovieDetailDto
            {
                Id = movie.ID,
                Title = movie.Title,
                Description = movie.Description ?? string.Empty,
                ReleaseYear = movie.ReleaseYear,
                IsEditorsChoice = movie.IsEditorsChoice,
                IsNewRelease = movie.IsNewRelease,
                IsAwardWinner = movie.IsAwardWinner,
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
                AverageRating = avg,
                ReviewCount = count,
                Actors = actorNames,
                Directors = directorNames,
                Awards = new List<MovieAwardInfoDto>()
            };
        }

        public async Task<bool> DeleteMovie(int id)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(id);
            if (movie == null)
                return false;

            movie.Status = Domain.Enums.MovieStatus.Inactive;

            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        public async Task<bool> UpdateMovie(UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetMovieWithCategoryAsync(dto.Id);
            if (movie == null)
                return false;

            if (dto.CategoryIds == null || dto.CategoryIds.Count == 0)
                return false;

            var distinctCategoryIds = dto.CategoryIds.Where(x => x > 0).Distinct().ToList();
            if (distinctCategoryIds.Count == 0)
                return false;

            foreach (var cid in distinctCategoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(cid);
                if (category == null)
                    return false;
            }

            var actorIds = (dto.ActorIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();
            var directorIds = (dto.DirectorIds ?? new List<int>()).Where(x => x > 0).Distinct().ToList();

            var createdActorIds = await EnsureActorsAsync(dto.NewActors);
            var createdDirectorIds = await EnsureDirectorsAsync(dto.NewDirectors);

            actorIds = actorIds.Concat(createdActorIds).Distinct().ToList();
            directorIds = directorIds.Concat(createdDirectorIds).Distinct().ToList();

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
            movie.IsAwardWinner = dto.IsAwardWinner;

            movie.MovieCategories ??= new List<MovieCategory>();
            var existingCategoryIds = movie.MovieCategories.Select(x => x.CategoryId).Distinct().ToList();

            var removeCategories = movie.MovieCategories.Where(x => !distinctCategoryIds.Contains(x.CategoryId)).ToList();
            foreach (var rm in removeCategories)
                movie.MovieCategories.Remove(rm);

            var addCategories = distinctCategoryIds.Where(x => !existingCategoryIds.Contains(x)).ToList();
            foreach (var cid in addCategories)
            {
                movie.MovieCategories.Add(new MovieCategory
                {
                    CategoryId = cid,
                    MovieId = movie.ID
                });
            }

            movie.MovieActors ??= new List<MovieActor>();
            var existingActorIds = movie.MovieActors.Select(x => x.ActorId).Distinct().ToList();

            var removeActors = movie.MovieActors.Where(x => !actorIds.Contains(x.ActorId)).ToList();
            foreach (var rm in removeActors)
                movie.MovieActors.Remove(rm);

            var addActors = actorIds.Where(x => !existingActorIds.Contains(x)).ToList();
            foreach (var aid in addActors)
            {
                movie.MovieActors.Add(new MovieActor
                {
                    MovieId = movie.ID,
                    ActorId = aid
                });
            }

            movie.MovieDirectors ??= new List<MovieDirector>();
            var existingDirectorIds = movie.MovieDirectors.Select(x => x.DirectorId).Distinct().ToList();

            var removeDirectors = movie.MovieDirectors.Where(x => !directorIds.Contains(x.DirectorId)).ToList();
            foreach (var rm in removeDirectors)
                movie.MovieDirectors.Remove(rm);

            var addDirectors = directorIds.Where(x => !existingDirectorIds.Contains(x)).ToList();
            foreach (var did in addDirectors)
            {
                movie.MovieDirectors.Add(new MovieDirector
                {
                    MovieId = movie.ID,
                    DirectorId = did
                });
            }

            await _movieRepository.UpdateAsync(movie);
            return true;
        }

        private async Task<List<int>> EnsureActorsAsync(string? newActors)
        {
            var ids = new List<int>();
            var names = SplitNames(newActors);

            foreach (var fullName in names)
            {
                var (first, last) = SplitFirstLast(fullName);

                var existing = await _actorRepository.FindByNameAsync(first, last);
                if (existing != null)
                {
                    ids.Add(existing.ID);
                    continue;
                }

                var actor = new Actor
                {
                    FirstName = first,
                    LastName = last,
                    Biography = null
                };

                var added = await _actorRepository.AddAsync(actor);
                ids.Add(added.ID);
            }

            return ids;
        }

        private async Task<List<int>> EnsureDirectorsAsync(string? newDirectors)
        {
            var ids = new List<int>();
            var names = SplitNames(newDirectors);

            foreach (var fullName in names)
            {
                var (first, last) = SplitFirstLast(fullName);

                var existing = await _directorRepository.FindByNameAsync(first, last);
                if (existing != null)
                {
                    ids.Add(existing.ID);
                    continue;
                }

                var director = new Director
                {
                    FirstName = first,
                    LastName = last,
                    Biography = null
                };

                var added = await _directorRepository.AddAsync(director);
                ids.Add(added.ID);
            }

            return ids;
        }

        private static List<string> SplitNames(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return new List<string>();

            return raw
                .Split(new[] { ',', '\n', '\r', ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static (string First, string Last) SplitFirstLast(string fullName)
        {
            var n = (fullName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(n))
                return ("-", "-");

            var parts = n.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1)
                return (parts[0], "-");

            var last = parts[^1];
            var first = string.Join(' ', parts.Take(parts.Length - 1));

            if (string.IsNullOrWhiteSpace(first)) first = "-";
            if (string.IsNullOrWhiteSpace(last)) last = "-";

            return (first, last);
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
                        IsAwardWinner = m.IsAwardWinner,
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
