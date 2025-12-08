using Application.DTOs.MovieDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class MovieServiceManager
    {
        private readonly IMovieRepository _movieRepository;

        public MovieServiceManager(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
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
                    CategoryName = m.Category != null
                        ? m.Category.CategoryName
                        : null
                })
                .ToList();
        }

        // EKLEME
        public async Task AddMovie(CreateMovieDto dto)
        {
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
        }

        // TEK GETIRME (Api de güncel versiyonu çağırmak için UpdateMovieDto kullanıyoruz)
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

        // Film detayını (MovieDetailDto) dönen metot
        public async Task<MovieDetailDto?> GetMovieDetailAsync(int id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);
            if (movie == null)
                return null;

            // Kategori adı dolu gelmeyebilir, varsa dolduruyoruz
            var categoryName = movie.Category != null
                ? movie.Category.CategoryName
                : string.Empty;

            var dto = new MovieDetailDto
            {
                Id = movie.ID,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                CategoryId = movie.CategoryId,
                CategoryName = categoryName,

                // Bu alanlar Movie entity’sinde henüz olmayabilir,
                // ileride Movie genişletildiğinde dolduracağız.
                OriginalTitle = null,
                TechnicalDetails = null,
                AudioFeatures = null,
                SubtitleLanguages = null,
                TrailerUrl = null,
                CoverImageUrl = null,

                // Oyuncu, Yönetmen, Ödül listeleri şu an boş kalıyor.
                // Metadata entity’leri ve ilişkileri tamamlandığında dolduracağız.
                Actors = new List<string>(),
                Directors = new List<string>(),
                Awards = new List<MovieAwardInfoDto>()
            };

            return dto;
        }


        // SILME
        public async Task DeleteMovie(UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(dto.Id);
            if (movie != null)
            {
                await _movieRepository.DeleteAsync(movie);
            }
        }

        // GUNCELLEME
        public async Task<bool> UpdateMovie(UpdateMovieDto dto)
        {
            var movie = await _movieRepository.GetByIdAsync(dto.Id);

            // Film bulunamadıysa false dön
            if (movie == null)
                return false;

            // Güncelleme
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

            return true; // Başarılı
        }


    }
}
