using Application.DTOs.WarehouseDTOs;
using Application.Repositories;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.ServiceManager
{
    public class WarehouseServiceManager
    {
        private readonly IMovieCopyRepository _movieCopyRepository;
        private readonly IShelfRepository _shelfRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IDamagedMovieRepository _damagedMovieRepository;

        public WarehouseServiceManager(
            IMovieCopyRepository movieCopyRepository,
            IShelfRepository shelfRepository,
            IMovieRepository movieRepository,
            IDamagedMovieRepository damagedMovieRepository)
        {
            _movieCopyRepository = movieCopyRepository;
            _shelfRepository = shelfRepository;
            _movieRepository = movieRepository;
            _damagedMovieRepository = damagedMovieRepository;
        }

        // 1) Raf bazlı envanter listesini getir
        public async Task<List<ShelfInventoryItemDto>> GetShelfInventoryAsync(int shelfId)
        {
            var copies = await _movieCopyRepository.GetAllAsync(c => c.ShelfId == shelfId);

            if (!copies.Any())
                return new List<ShelfInventoryItemDto>();

            var movieIds = copies.Select(c => c.MovieId).Distinct().ToList();
            var movies = await _movieRepository.GetAllAsync(m => movieIds.Contains(m.ID));

            var shelf = await _shelfRepository.GetByIdAsync(shelfId);

            return copies
                .Select(c =>
                {
                    var movie = movies.FirstOrDefault(m => m.ID == c.MovieId);

                    return new ShelfInventoryItemDto
                    {
                        MovieCopyId = c.ID,
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

        // 2) Kopyayı belirli bir rafa taşı
        public async Task<bool> MoveCopyToShelfAsync(MoveCopyToShelfDto dto)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(dto.MovieCopyId);
            if (copy == null)
                return false;

            var shelf = await _shelfRepository.GetByIdAsync(dto.ShelfId);
            if (shelf == null)
                return false;

            copy.ShelfId = dto.ShelfId;

            await _movieCopyRepository.UpdateAsync(copy);
            return true;
        }

        // 3) İade gelen filmi işle (bozuk / sağlam)
        public async Task<bool> ProcessReturnAsync(ProcessReturnDto dto)
        {
            var copy = await _movieCopyRepository.GetByIdAsync(dto.MovieCopyId);
            if (copy == null)
                return false;

            // Kopya iade geldiğinde artık depoda mevcut kabul edilir
            copy.IsAvailable = !dto.IsDamaged;
            copy.IsDamaged = dto.IsDamaged;

            // Eğer bozuksa DamagedMovie kaydı oluştur
            if (dto.IsDamaged)
            {
                var damaged = new DamagedMovie
                {
                    MovieCopyId = copy.ID,
                    Note = dto.Note,
                    IsSentToPurchase = false
                };

                await _damagedMovieRepository.AddAsync(damaged);
            }

            // Raf güncellemesi yapmak istiyorsak
            if (dto.TargetShelfId.HasValue)
            {
                var shelf = await _shelfRepository.GetByIdAsync(dto.TargetShelfId.Value);
                if (shelf != null)
                {
                    copy.ShelfId = dto.TargetShelfId.Value;
                }
            }

            await _movieCopyRepository.UpdateAsync(copy);
            return true;
        }
    }
}
