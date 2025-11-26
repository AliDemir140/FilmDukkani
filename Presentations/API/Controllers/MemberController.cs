using Application.DTOs.MemberDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly MemberServiceManager _memberServiceManager;

        public MemberController(MemberServiceManager memberServiceManager)
        {
            _memberServiceManager = memberServiceManager;
        }

        // GET: api/member/members
        [HttpGet("members")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _memberServiceManager.GetMembersAsync();
            return Ok(members);
        }

        // GET: api/member/get-member?id=5
        [HttpGet("get-member")]
        public async Task<IActionResult> GetMember(int id)
        {
            var member = await _memberServiceManager.GetMember(id);
            if (member == null)
                return NotFound("Üye bulunamadı.");

            return Ok(member);
        }

        // POST: api/member/add-member
        [HttpPost("add-member")]
        public async Task<IActionResult> AddMember([FromBody] CreateMemberDto memberDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _memberServiceManager.AddMember(memberDto);
            return Ok("Üye eklendi.");
        }

        // PUT: api/member/update-member
        [HttpPut("update-member")]
        public async Task<IActionResult> UpdateMember([FromBody] UpdateMemberDto updateMember)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _memberServiceManager.UpdateMember(updateMember);
            if (!result)
                return NotFound("Üye bulunamadı.");

            return Ok("Üye güncellendi.");
        }

        // DELETE: api/member/delete-member?id=5
        [HttpDelete("delete-member")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var result = await _memberServiceManager.DeleteMember(id);
            if (!result)
                return NotFound("Üye bulunamadı.");

            return Ok("Üye silindi.");
        }
    }
}
