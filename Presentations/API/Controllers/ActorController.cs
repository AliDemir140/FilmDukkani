using Application.DTOs.ActorDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorController : ControllerBase
    {
        private readonly ActorServiceManager _actorService;

        public ActorController(ActorServiceManager actorService)
        {
            _actorService = actorService;
        }

        // GET api/Actor/actors
        [HttpGet("actors")]
        public async Task<IActionResult> GetActors()
        {
            var actors = await _actorService.GetActorsAsync();
            return Ok(actors);
        }

        // POST api/Actor/add-actor
        [HttpPost("add-actor")]
        public async Task<IActionResult> AddActor([FromBody] CreateActorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _actorService.AddActorAsync(dto);
            return Ok("Oyuncu eklendi.");
        }

        // GET api/Actor/get-actor?id=1
        [HttpGet("get-actor")]
        public async Task<IActionResult> GetActor(int id)
        {
            var actor = await _actorService.GetActorAsync(id);
            if (actor == null)
                return NotFound("Oyuncu bulunamadı.");

            return Ok(actor);
        }

        // PUT api/Actor/update-actor
        [HttpPut("update-actor")]
        public async Task<IActionResult> UpdateActor([FromBody] UpdateActorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _actorService.UpdateActorAsync(dto);
            if (!result)
                return NotFound("Oyuncu bulunamadı.");

            return Ok("Oyuncu güncellendi.");
        }

        // DELETE api/Actor/delete-actor?id=1
        [HttpDelete("delete-actor")]
        public async Task<IActionResult> DeleteActor(int id)
        {
            var result = await _actorService.DeleteActorAsync(id);
            if (!result)
                return NotFound("Oyuncu bulunamadı.");

            return Ok("Oyuncu silindi.");
        }
    }
}
