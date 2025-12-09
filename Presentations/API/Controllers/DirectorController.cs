using Application.DTOs.DirectorDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorController : ControllerBase
    {
        private readonly DirectorServiceManager _directorService;

        public DirectorController(DirectorServiceManager directorService)
        {
            _directorService = directorService;
        }

        [HttpGet("directors")]
        public async Task<IActionResult> GetDirectors()
        {
            var directors = await _directorService.GetDirectorsAsync();
            return Ok(directors);
        }

        [HttpPost("add-director")]
        public async Task<IActionResult> AddDirector([FromBody] CreateDirectorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _directorService.AddDirectorAsync(dto);
            return Ok("Yönetmen eklendi.");
        }

        [HttpGet("get-director")]
        public async Task<IActionResult> GetDirector(int id)
        {
            var director = await _directorService.GetDirectorAsync(id);
            if (director == null)
                return NotFound("Yönetmen bulunamadı.");

            return Ok(director);
        }

        [HttpPut("update-director")]
        public async Task<IActionResult> UpdateDirector([FromBody] UpdateDirectorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _directorService.UpdateDirectorAsync(dto);
            if (!result)
                return NotFound("Yönetmen bulunamadı.");

            return Ok("Yönetmen güncellendi.");
        }

        [HttpDelete("delete-director")]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var result = await _directorService.DeleteDirectorAsync(id);
            if (!result)
                return NotFound("Yönetmen bulunamadı.");

            return Ok("Yönetmen silindi.");
        }
    }
}
