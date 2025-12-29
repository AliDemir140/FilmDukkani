using Application.DTOs.DirectorDTOs;
using Application.DTOs.PersonDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class DirectorServiceManager
    {
        private readonly IDirectorRepository _directorRepository;

        public DirectorServiceManager(IDirectorRepository directorRepository)
        {
            _directorRepository = directorRepository;
        }

        public async Task<List<DirectorDto>> GetDirectorsAsync()
        {
            var directors = await _directorRepository.GetAllAsync();

            return directors
                .Select(d => new DirectorDto
                {
                    Id = d.ID,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    Biography = d.Biography,
                    CreatedDate = d.CreatedDate,
                    ModifiedDate = d.ModifiedDate
                })
                .ToList();
        }

        public async Task AddDirectorAsync(CreateDirectorDto dto)
        {
            var director = new Director
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Biography = dto.Biography
            };

            await _directorRepository.AddAsync(director);
        }

        public async Task<UpdateDirectorDto?> GetDirectorAsync(int id)
        {
            var director = await _directorRepository.GetByIdAsync(id);
            if (director == null)
                return null;

            return new UpdateDirectorDto
            {
                Id = director.ID,
                FirstName = director.FirstName,
                LastName = director.LastName,
                Biography = director.Biography
            };
        }

        public async Task<bool> UpdateDirectorAsync(UpdateDirectorDto dto)
        {
            var director = await _directorRepository.GetByIdAsync(dto.Id);
            if (director == null)
                return false;

            director.FirstName = dto.FirstName;
            director.LastName = dto.LastName;
            director.Biography = dto.Biography;

            await _directorRepository.UpdateAsync(director);
            return true;
        }

        public async Task<bool> DeleteDirectorAsync(int id)
        {
            var director = await _directorRepository.GetByIdAsync(id);
            if (director == null)
                return false;

            await _directorRepository.DeleteAsync(director);
            return true;
        }

        public async Task<List<PersonLookupDto>> GetDirectorsForSelectAsync()
        {
            var list = await _directorRepository.GetAllAsNoTrackingAsync();

            return list
                .Select(d => new PersonLookupDto
                {
                    Id = d.ID,
                    FullName = $"{d.FirstName} {d.LastName}".Trim()
                })
                .OrderBy(x => x.FullName)
                .ToList();
        }
    }
}
