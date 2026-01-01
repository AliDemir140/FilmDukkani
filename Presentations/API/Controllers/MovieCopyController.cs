using Application.DTOs.MovieCopyDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "WarehouseAccess")]
    public class MovieCopyController : ControllerBase
    {
        private readonly MovieCopyServiceManager _movieCopyService;

        public MovieCopyController(MovieCopyServiceManager movieCopyService)
        {
            _movieCopyService = movieCopyService;
        }

        [HttpGet("movie-copies")]
        public async Task<IActionResult> GetMovieCopies()
        {
            var copies = await _movieCopyService.GetMovieCopiesAsync();
            return Ok(copies);
        }

        [HttpGet("get-movie-copy")]
        public async Task<IActionResult> GetMovieCopy(int id)
        {
            var copy = await _movieCopyService.GetMovieCopyAsync(id);
            if (copy == null)
                return NotFound("Film kopyası bulunamadı.");

            return Ok(copy);
        }

        [HttpPost("add-movie-copy")]
        public async Task<IActionResult> AddMovieCopy([FromBody] CreateMovieCopyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.IsAvailable && dto.IsDamaged)
                return BadRequest("Film kopyası aynı anda hem Uygun hem Hasarlı olamaz.");

            if (dto.IsDamaged)
                dto.IsAvailable = false;

            var (ok, error) = await _movieCopyService.AddMovieCopyAsync(dto);

            if (!ok)
                return BadRequest(error);

            return Ok("Film kopyası eklendi.");
        }

        [HttpPut("update-movie-copy")]
        public async Task<IActionResult> UpdateMovieCopy([FromBody] UpdateMovieCopyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.IsAvailable && dto.IsDamaged)
                return BadRequest("Film kopyası aynı anda hem Uygun hem Hasarlı olamaz.");

            if (dto.IsDamaged)
                dto.IsAvailable = false;

            var (ok, error) = await _movieCopyService.UpdateMovieCopyAsync(dto);

            if (!ok)
                return BadRequest(error);

            return Ok("Film kopyası güncellendi.");
        }

        [HttpDelete("delete-movie-copy")]
        public async Task<IActionResult> DeleteMovieCopy(int id)
        {
            var ok = await _movieCopyService.DeleteMovieCopyAsync(id);
            if (!ok)
                return NotFound("Film kopyası bulunamadı.");

            return Ok("Film kopyası silindi.");
        }
    }
}
