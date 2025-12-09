using Application.DTOs.AwardDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AwardController : ControllerBase
    {
        private readonly AwardServiceManager _awardService;

        public AwardController(AwardServiceManager awardService)
        {
            _awardService = awardService;
        }

        [HttpGet("awards")]
        public async Task<IActionResult> GetAwards()
        {
            var awards = await _awardService.GetAwardsAsync();
            return Ok(awards);
        }

        [HttpPost("add-award")]
        public async Task<IActionResult> AddAward([FromBody] CreateAwardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _awardService.AddAwardAsync(dto);
            return Ok("Ödül eklendi.");
        }

        [HttpGet("get-award")]
        public async Task<IActionResult> GetAward(int id)
        {
            var award = await _awardService.GetAwardAsync(id);
            if (award == null)
                return NotFound("Ödül bulunamadı.");

            return Ok(award);
        }

        [HttpPut("update-award")]
        public async Task<IActionResult> UpdateAward([FromBody] UpdateAwardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _awardService.UpdateAwardAsync(dto);
            if (!result)
                return NotFound("Ödül bulunamadı.");

            return Ok("Ödül güncellendi.");
        }

        [HttpDelete("delete-award")]
        public async Task<IActionResult> DeleteAward(int id)
        {
            var result = await _awardService.DeleteAwardAsync(id);
            if (!result)
                return NotFound("Ödül bulunamadı.");

            return Ok("Ödül silindi.");
        }
    }
}
