using Application.DTOs.MemberDTOs;
using Application.Repositories;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberProfileController : ControllerBase
    {
        private readonly IMemberRepository _memberRepository;
        private readonly IMembershipPlanRepository _membershipPlanRepository;

        public MemberProfileController(
            IMemberRepository memberRepository,
            IMembershipPlanRepository membershipPlanRepository)
        {
            _memberRepository = memberRepository;
            _membershipPlanRepository = membershipPlanRepository;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(int memberId)
        {
            if (memberId <= 0)
                return BadRequest(new { message = "memberId zorunludur." });

            var member = await _memberRepository.GetByIdAsync(memberId);
            if (member == null)
                return NotFound(new { message = "Üye bulunamadı." });

            var plan = await _membershipPlanRepository.GetByIdAsync(member.MembershipPlanId);

            var dto = new MemberProfileDto
            {
                MemberId = member.ID,
                FullName = (member.FirstName + " " + member.LastName).Trim(),
                Email = member.Email ?? string.Empty,
                MembershipPlanId = member.MembershipPlanId,
                MembershipPlanName = plan?.PlanName ?? string.Empty,
                MembershipStartDate = member.MembershipStartDate,
                Status = member.Status
            };

            return Ok(dto);
        }

        [HttpPut("update-plan")]
        public async Task<IActionResult> UpdatePlan([FromBody] UpdateMemberPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null)
                return NotFound(new { message = "Üye bulunamadı." });

            var plan = await _membershipPlanRepository.GetByIdAsync(dto.MembershipPlanId);
            if (plan == null)
                return NotFound(new { message = "Üyelik planı bulunamadı." });

            member.MembershipPlanId = dto.MembershipPlanId;

            if (member.MembershipStartDate == default)
                member.MembershipStartDate = DateTime.Today;

            await _memberRepository.UpdateAsync(member);

            return Ok(new { message = "Üyelik planı güncellendi." });
        }
    }
}
