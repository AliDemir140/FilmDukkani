using Application.DTOs.MembershipPlanDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembershipPlanController : ControllerBase
    {
        private readonly MembershipPlanServiceManager _membershipPlanServiceManager;

        public MembershipPlanController(MembershipPlanServiceManager membershipPlanServiceManager)
        {
            _membershipPlanServiceManager = membershipPlanServiceManager;
        }

        // GET: api/membershipplan/plans
        [HttpGet("plans")]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _membershipPlanServiceManager.GetPlansAsync();
            return Ok(plans);
        }

        // GET: api/membershipplan/get-plan?id=1
        [HttpGet("get-plan")]
        public async Task<IActionResult> GetPlan(int id)
        {
            var plan = await _membershipPlanServiceManager.GetPlan(id);
            if (plan == null)
                return NotFound("Üyelik planı bulunamadı.");

            return Ok(plan);
        }

        // POST: api/membershipplan/add-plan
        [HttpPost("add-plan")]
        public async Task<IActionResult> AddPlan([FromBody] CreateMembershipPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _membershipPlanServiceManager.AddPlan(dto);
            return Ok("Üyelik planı eklendi.");
        }

        // PUT: api/membershipplan/update-plan
        [HttpPut("update-plan")]
        public async Task<IActionResult> UpdatePlan([FromBody] UpdateMembershipPlanDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _membershipPlanServiceManager.UpdatePlan(dto);
            if (!result)
                return NotFound("Üyelik planı bulunamadı.");

            return Ok("Üyelik planı güncellendi.");
        }

        // DELETE: api/membershipplan/delete-plan?id=1
        [HttpDelete("delete-plan")]
        public async Task<IActionResult> DeletePlan(int id)
        {
            var result = await _membershipPlanServiceManager.DeletePlan(id);
            if (!result)
                return NotFound("Üyelik planı bulunamadı.");

            return Ok("Üyelik planı silindi.");
        }
    }
}
