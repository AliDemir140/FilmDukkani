using Application.DTOs.ActorDTOs;
using Application.Repositories;
using Domain.Entities;

namespace Application.ServiceManager
{
    public class ActorServiceManager
    {
        private readonly IActorRepository _actorRepository;

        public ActorServiceManager(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }

        // Listeleme
        public async Task<List<ActorDto>> GetActorsAsync()
        {
            var actors = await _actorRepository.GetAllAsync();

            return actors
                .Select(a => new ActorDto
                {
                    Id = a.ID,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Biography = a.Biography,
                    CreatedDate = a.CreatedDate,
                    ModifiedDate = a.ModifiedDate
                })
                .ToList();
        }

        // Ekleme
        public async Task AddActorAsync(CreateActorDto dto)
        {
            var actor = new Actor
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Biography = dto.Biography
            };

            await _actorRepository.AddAsync(actor);
        }

        // Tek getirme (update formu için)
        public async Task<UpdateActorDto?> GetActorAsync(int id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null)
                return null;

            return new UpdateActorDto
            {
                Id = actor.ID,
                FirstName = actor.FirstName,
                LastName = actor.LastName,
                Biography = actor.Biography
            };
        }

        // Güncelleme
        public async Task<bool> UpdateActorAsync(UpdateActorDto dto)
        {
            var actor = await _actorRepository.GetByIdAsync(dto.Id);
            if (actor == null)
                return false;

            actor.FirstName = dto.FirstName;
            actor.LastName = dto.LastName;
            actor.Biography = dto.Biography;

            await _actorRepository.UpdateAsync(actor);
            return true;
        }

        // Silme
        public async Task<bool> DeleteActorAsync(int id)
        {
            var actor = await _actorRepository.GetByIdAsync(id);
            if (actor == null)
                return false;

            await _actorRepository.DeleteAsync(actor);
            return true;
        }
    }
}
