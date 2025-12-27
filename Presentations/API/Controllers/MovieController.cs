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

        [HttpGet("movies")]
        public async Task<IActionResult> GetMovies([FromQuery] int? categoryId, [FromQuery] string? q)
        {
            var movies = await _movieServiceManager.SearchMoviesAsync(categoryId, q);
            return Ok(movies);
        }

        [HttpGet("showcase/editors-choice")]
        public async Task<IActionResult> GetEditorsChoice()
        {
            var movies = await _movieServiceManager.GetEditorsChoiceAsync();
            return Ok(movies);
        }

        [HttpGet("showcase/new-releases")]
        public async Task<IActionResult> GetNewReleases()
        {
            var movies = await _movieServiceManager.GetNewReleasesAsync();
            return Ok(movies);
        }

        [HttpGet("showcase/top-rented")]
        public async Task<IActionResult> GetTopRented([FromQuery] int take = 10)
        {
            var movies = await _movieServiceManager.GetTopRentedAsync(take);
            return Ok(movies);
        }

        [HttpGet("showcase/award-winners")]
        public async Task<IActionResult> GetAwardWinners([FromQuery] int take = 10)
        {
            var movies = await _movieServiceManager.GetAwardWinnersAsync(take);
            return Ok(movies);
        }


        [HttpPost("add-movie")]
        public async Task<IActionResult> AddMovie([FromBody] CreateMovieDto movieDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _movieServiceManager.AddMovie(movieDto);
            if (!ok)
                return BadRequest("Film eklenemedi.");

            return Ok("Film eklendi.");
        }

        [HttpGet("get-movie")]
        public async Task<IActionResult> GetMovie(int id)
        {
            var movie = await _movieServiceManager.GetMovie(id);
            if (movie == null)
                return NotFound("Film bulunamadı.");

            return Ok(movie);
        }

        [HttpDelete("delete-movie")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var result = await _movieServiceManager.DeleteMovie(id);

            if (!result)
                return NotFound("Film bulunamadı.");

            return Ok("Film silindi.");
        }

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

        [HttpGet("{id}/detail")]
        public async Task<IActionResult> GetMovieDetail(int id)
        {
            var movieDetail = await _movieServiceManager.GetMovieDetailAsync(id);
            if (movieDetail == null)
                return NotFound("Film bulunamadı.");

            return Ok(movieDetail);
        }
    }
}
