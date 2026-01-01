using Application.DTOs.PurchaseRequestDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestController : ControllerBase
    {
        private readonly PurchaseRequestServiceManager _service;

        public PurchaseRequestController(PurchaseRequestServiceManager service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePurchaseRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _service.CreateAsync(dto);
            if (!ok)
                return BadRequest("Talep oluşturulamadı.");

            return Ok("Talep oluşturuldu.");
        }

        [AllowAnonymous]
        [HttpGet("my")]
        public async Task<IActionResult> My([FromQuery] int memberId)
        {
            if (memberId <= 0)
                return BadRequest("memberId zorunludur.");

            var list = await _service.GetByMemberAsync(memberId);
            return Ok(list);
        }

        [Authorize(Policy = "PurchasingAccess")]
        [HttpGet("pending")]
        public async Task<IActionResult> Pending()
        {
            var list = await _service.GetPendingAsync();
            return Ok(list);
        }

        [Authorize(Policy = "PurchasingAccess")]
        [HttpPost("decide")]
        public async Task<IActionResult> Decide([FromBody] DecidePurchaseRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _service.DecideAsync(dto);
            if (!ok)
                return BadRequest("Karar işlemi başarısız.");

            return Ok("Karar kaydedildi.");
        }
    }
}
