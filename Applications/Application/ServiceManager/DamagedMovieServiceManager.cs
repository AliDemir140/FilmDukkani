using Application.DTOs.DamagedMovieDTOs;
using Application.Repositories;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ServiceManager
{
    public class DamagedMovieServiceManager
    {
        private readonly IDamagedMovieRepository _damagedMovieRepository;
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IMovieRepository _movieRepository;

        public DamagedMovieServiceManager(
            IDamagedMovieRepository damagedMovieRepository,
            IMovieCopyRepository movieCopyRepository,
            IMovieRepository movieRepository)
        {
            _damagedMovieRepository = damagedMovieRepository;
            _movieCopyRepository = movieCopyRepository;
            _movieRepository = movieRepository;
        }

        // Tüm bozuk film kayıtlarını listele
        public async Task<List<DamagedMovieDto>> GetDamagedMoviesAsync()
        {
            var damagedList = await _damagedMovieRepository.GetAllAsync();

            var copyIds = damagedList.Select(d => d.MovieCopyId).Distinct().ToList();
            var copies = await _movieCopyRepository.GetAllAsync(c => copyIds.Contains(c.ID));

            var movieIds = copies.Select(c => c.MovieId).Distinct().ToList();
            var movies = await _movieRepository.GetAllAsync(m => movieIds.Contains(m.ID));

            return damagedList
                .Select(d =>
                {
                    var copy = copies.FirstOrDefault(c => c.ID == d.MovieCopyId);
                    var movie = copy != null
                        ? movies.FirstOrDefault(m => m.ID == copy.MovieId)
                        : null;

                    return new DamagedMovieDto
                    {
                        Id = d.ID,
                        MovieCopyId = d.MovieCopyId,
                        MovieTitle = movie?.Title ?? string.Empty,
                        Barcode = copy?.Barcode ?? string.Empty,
                        Note = d.Note,
                        IsSentToPurchase = d.IsSentToPurchase,
                        CreatedDate = d.CreatedDate,
                        ModifiedDate = d.ModifiedDate
                    };
                })
                .ToList();
        }

        // Tek kayıt getir (update ekranı için)
        public async Task<UpdateDamagedMovieDto?> GetDamagedMovieAsync(int id)
        {
            var damaged = await _damagedMovieRepository.GetByIdAsync(id);
            if (damaged == null)
                return null;

            return new UpdateDamagedMovieDto
            {
                Id = damaged.ID,
                MovieCopyId = damaged.MovieCopyId,
                Note = damaged.Note,
                IsSentToPurchase = damaged.IsSentToPurchase
            };
        }

        // Yeni bozuk film kaydı ekle
        public async Task AddDamagedMovieAsync(CreateDamagedMovieDto dto)
        {
            var entity = new DamagedMovie
            {
                MovieCopyId = dto.MovieCopyId,
                Note = dto.Note,
                IsSentToPurchase = false
            };

            await _damagedMovieRepository.AddAsync(entity);
        }

        // Bozuk film kaydını güncelle
        public async Task<bool> UpdateDamagedMovieAsync(UpdateDamagedMovieDto dto)
        {
            var damaged = await _damagedMovieRepository.GetByIdAsync(dto.Id);
            if (damaged == null)
                return false;

            damaged.MovieCopyId = dto.MovieCopyId;
            damaged.Note = dto.Note;
            damaged.IsSentToPurchase = dto.IsSentToPurchase;

            await _damagedMovieRepository.UpdateAsync(damaged);
            return true;
        }

        // Kayıt sil
        public async Task<bool> DeleteDamagedMovieAsync(int id)
        {
            var damaged = await _damagedMovieRepository.GetByIdAsync(id);
            if (damaged == null)
                return false;

            await _damagedMovieRepository.DeleteAsync(damaged);
            return true;
        }

        // Belirli bir MovieCopy'ye ait bozuk kayıtları getir
        public async Task<List<DamagedMovieDto>> GetDamagedMoviesByCopyAsync(int movieCopyId)
        {
            var damagedList = await _damagedMovieRepository
                .GetAllAsync(d => d.MovieCopyId == movieCopyId);

            if (!damagedList.Any())
                return new List<DamagedMovieDto>();

            var copy = await _movieCopyRepository.GetByIdAsync(movieCopyId);
            var movie = copy != null
                ? await _movieRepository.GetByIdAsync(copy.MovieId)
                : null;

            return damagedList
                .Select(d => new DamagedMovieDto
                {
                    Id = d.ID,
                    MovieCopyId = d.MovieCopyId,
                    MovieTitle = movie?.Title ?? string.Empty,
                    Barcode = copy?.Barcode ?? string.Empty,
                    Note = d.Note,
                    IsSentToPurchase = d.IsSentToPurchase,
                    CreatedDate = d.CreatedDate,
                    ModifiedDate = d.ModifiedDate
                })
                .ToList();
        }
    }
}
