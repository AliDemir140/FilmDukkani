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
                    Description = m.Description,
                    ReleaseYear = m.ReleaseYear,
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
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear,
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
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                CategoryId = movie.CategoryId
            };
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
            movie.Description = dto.Description;
            movie.ReleaseYear = dto.ReleaseYear;
            movie.CategoryId = dto.CategoryId;

            await _movieRepository.UpdateAsync(movie);

            return true; // Başarılı
        }

    }
}
