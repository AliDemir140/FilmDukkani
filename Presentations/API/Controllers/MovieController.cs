using Application.DTOs.MovieDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieServiceManager _movieServiceManager;

        public MovieController(MovieServiceManager movieServiceManager)
        {
            _movieServiceManager = movieServiceManager;
        }

        // GET: api/movie/movies
        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies()
        {
            var movies = await _movieServiceManager.GetMoviesAsync();
            return Ok(movies);
        }

        // POST: api/movie/add-movie
        [HttpPost("add-movie")]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _movieServiceManager.AddMovie(movieDto);
            return Ok("Film eklendi.");
        }

        // GET: api/movie/get-movie?id=5
        [HttpGet("get-movie")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _movieServiceManager.GetMovie(id);
            if (movie == null)
                return NotFound("Film bulunamadı.");

            return Ok(movie);
        }

        // DELETE: api/movie/delete-movie?id=5
        [HttpDelete("delete-movie")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _movieServiceManager.GetMovie(id);
            if (movie == null)
                return NotFound("Film bulunamadı.");

            await _movieServiceManager.DeleteMovie(movie);
            return Ok("Film silindi.");
        }

        // PUT: api/movie/update-movie
        [HttpPut("update-movie")]
        public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieDto updateMovie)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _movieServiceManager.UpdateMovie(updateMovie);

            if (!result)
                return NotFound("Film bulunamadı.");

            return Ok("Film güncellendi.");
        }

    }
}
