using Application.DTOs.DamagedMovieDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamagedMovieController : ControllerBase
    {
        private readonly DamagedMovieServiceManager _damagedService;

        public DamagedMovieController(DamagedMovieServiceManager damagedService)
        {
            _damagedService = damagedService;
        }

        // GET api/DamagedMovie/damaged-movies
        [HttpGet("damaged-movies")]
        public async Task<IActionResult> GetDamagedMovies()
        {
            var list = await _damagedService.GetDamagedMoviesAsync();
            return Ok(list);
        }

        // GET api/DamagedMovie/get-damaged-movie?id=1
        [HttpGet("get-damaged-movie")]
        public async Task<IActionResult> GetDamagedMovie(int id)
        {
            var damaged = await _damagedService.GetDamagedMovieAsync(id);
            if (damaged == null)
                return NotFound("Bozuk film kaydı bulunamadı.");

            return Ok(damaged);
        }

        // GET api/DamagedMovie/damaged-movies-by-copy?movieCopyId=1
        [HttpGet("damaged-movies-by-copy")]
        public async Task<IActionResult> GetDamagedMoviesByCopy(int movieCopyId)
        {
            var list = await _damagedService.GetDamagedMoviesByCopyAsync(movieCopyId);
            return Ok(list);
        }

        // POST api/DamagedMovie/add-damaged-movie
        [HttpPost("add-damaged-movie")]
        public async Task<IActionResult> AddDamagedMovie([FromBody] CreateDamagedMovieDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _damagedService.AddDamagedMovieAsync(dto);
            return Ok("Bozuk film kaydı eklendi.");
        }

        // PUT api/DamagedMovie/update-damaged-movie
        [HttpPut("update-damaged-movie")]
        public async Task<IActionResult> UpdateDamagedMovie([FromBody] UpdateDamagedMovieDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _damagedService.UpdateDamagedMovieAsync(dto);
            if (!result)
                return NotFound("Bozuk film kaydı bulunamadı.");

            return Ok("Bozuk film kaydı güncellendi.");
        }

        // DELETE api/DamagedMovie/delete-damaged-movie?id=1
        [HttpDelete("delete-damaged-movie")]
        public async Task<IActionResult> DeleteDamagedMovie(int id)
        {
            var result = await _damagedService.DeleteDamagedMovieAsync(id);
            if (!result)
                return NotFound("Bozuk film kaydı bulunamadı.");

            return Ok("Bozuk film kaydı silindi.");
        }
    }
}
