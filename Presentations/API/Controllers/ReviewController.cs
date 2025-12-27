using Application.DTOs.ReviewDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ReviewServiceManager _reviewService;

        public ReviewController(ReviewServiceManager reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("movie/{movieId}")]
        public async Task<IActionResult> GetByMovie(int movieId, [FromQuery] int? currentMemberId)
        {
            if (movieId <= 0) return BadRequest("movieId zorunludur.");

            var list = await _reviewService.GetMovieReviewsAsync(movieId, currentMemberId);
            return Ok(list);
        }

        [HttpGet("movie/{movieId}/summary")]
        public async Task<IActionResult> GetSummary(int movieId)
        {
            if (movieId <= 0) return BadRequest("movieId zorunludur.");

            var (avg, count) = await _reviewService.GetMovieRatingSummaryAsync(movieId);
            return Ok(new { Average = avg, Count = count });
        }

        [HttpPost("add-or-update")]
        public async Task<IActionResult> AddOrUpdate([FromBody] CreateReviewDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _reviewService.AddOrUpdateAsync(dto);
            if (id == 0) return BadRequest("Geçersiz istek.");
            if (id == -1) return NotFound("Film bulunamadı.");
            if (id == -2) return NotFound("Üye bulunamadı.");

            return Ok(new { ReviewId = id });
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> Delete(int reviewId, [FromQuery] int memberId)
        {
            if (reviewId <= 0 || memberId <= 0) return BadRequest("reviewId ve memberId zorunludur.");

            var ok = await _reviewService.DeleteAsync(reviewId, memberId);
            if (!ok) return BadRequest("Silme işlemi yapılamadı.");

            return Ok("Yorum silindi.");
        }
    }
}
