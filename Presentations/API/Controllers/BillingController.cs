using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using BillingServiceManager = Application.ServiceManager.BillingServiceManager;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        private readonly BillingServiceManager _billingServiceManager;
        private readonly IConfiguration _configuration;

        public BillingController(BillingServiceManager billingServiceManager, IConfiguration configuration)
        {
            _billingServiceManager = billingServiceManager;
            _configuration = configuration;
        }

        public class ChargeMembershipDto
        {
            [Required]
            public int MemberId { get; set; }
        }

        [HttpPost("charge-now")]
        public async Task<IActionResult> ChargeNow([FromBody] ChargeMembershipDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminEmail =
                _configuration["Billing:AdminEmail"]
                ?? _configuration["SeedAdmin:Email"]
                ?? string.Empty;

            var period = DateTime.Today.ToString("yyyy-MM");

            var result = await _billingServiceManager.ChargeNowAsync(dto.MemberId, period, adminEmail);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }
    }
}
