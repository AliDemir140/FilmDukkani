using Application.DTOs.MovieCopyDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieCopyController : ControllerBase
    {
        private readonly MovieCopyServiceManager _movieCopyService;

        public MovieCopyController(MovieCopyServiceManager movieCopyService)
        {
            _movieCopyService = movieCopyService;
        }

        // GET api/MovieCopy/movie-copies
        [HttpGet("movie-copies")]
        public async Task<IActionResult> GetMovieCopies()
        {
            var copies = await _movieCopyService.GetMovieCopiesAsync();
            return Ok(copies);
        }

        // GET api/MovieCopy/get-movie-copy?id=1
        [HttpGet("get-movie-copy")]
        public async Task<IActionResult> GetMovieCopy(int id)
        {
            var copy = await _movieCopyService.GetMovieCopyAsync(id);
            if (copy == null)
                return NotFound("Film kopyası bulunamadı.");

            return Ok(copy);
        }

        // POST api/MovieCopy/add-movie-copy
        [HttpPost("add-movie-copy")]
        public async Task<IActionResult> AddMovieCopy([FromBody] CreateMovieCopyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _movieCopyService.AddMovieCopyAsync(dto);
            return Ok("Film kopyası eklendi.");
        }

        // PUT api/MovieCopy/update-movie-copy
        [HttpPut("update-movie-copy")]
        public async Task<IActionResult> UpdateMovieCopy([FromBody] UpdateMovieCopyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _movieCopyService.UpdateMovieCopyAsync(dto);
            if (!result)
                return NotFound("Film kopyası bulunamadı.");

            return Ok("Film kopyası güncellendi.");
        }

        // DELETE api/MovieCopy/delete-movie-copy?id=1
        [HttpDelete("delete-movie-copy")]
        public async Task<IActionResult> DeleteMovieCopy(int id)
        {
            var result = await _movieCopyService.DeleteMovieCopyAsync(id);
            if (!result)
                return NotFound("Film kopyası bulunamadı.");

            return Ok("Film kopyası silindi.");
        }
    }
}
