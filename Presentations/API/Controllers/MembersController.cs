using Application.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;

        public MembersController(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        // GET: api/members/by-user/{userId}
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetMemberIdByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "userId zorunludur." });

            // tek kaydı bul
            var members = await _memberRepository.GetAllAsync(x => x.IdentityUserId == userId);
            var member = members.FirstOrDefault();

            if (member == null)
                return NotFound(new { message = "Member bulunamadı." });

            return Ok(member.ID);
        }
    }
}
