using Application.DTOs.DeliveryRequestDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryRequestController : ControllerBase
    {
        private readonly DeliveryRequestServiceManager _deliveryService;

        public DeliveryRequestController(DeliveryRequestServiceManager deliveryService)
        {
            _deliveryService = deliveryService;
        }

        // Teslimat isteği oluştur
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newId = await _deliveryService.CreateDeliveryRequestAsync(dto);

            if (newId == 0)
                return BadRequest("Geçersiz teslimat tarihi. Teslimat en az 2 gün sonrası olmalı ve Pazar günü seçilemez.");

            return Ok(new
            {
                Message = "Teslimat isteği oluşturuldu.",
                RequestId = newId
            });
        }

        // Yarının teslimatlarını hazırla
        [HttpPost("prepare-tomorrow")]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return Ok("Yarının teslimatları hazırlandı.");
        }

        // Teslimat isteği detay
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var request = await _deliveryService.GetRequestDetailAsync(id);
            if (request == null)
                return NotFound("Teslimat isteği bulunamadı.");

            return Ok(request);
        }

        // Teslimat iptal
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _deliveryService.CancelRequestAsync(id);
            if (!result)
                return BadRequest("Teslimat isteği iptal edilemedi.");

            return Ok("Teslimat isteği iptal edildi.");
        }

        // Teslim edildi (listeden düşer, süreç kapanır)
        [HttpPut("{id}/mark-delivered")]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var result = await _deliveryService.MarkDeliveredAsync(id);
            if (!result)
                return BadRequest("Teslimat isteği teslim edildi olarak işaretlenemedi.");

            return Ok("Teslimat teslim edildi ve süreç tamamlandı.");
        }

        // Film iade işlemi
        [HttpPost("return-item")]
        public async Task<IActionResult> ReturnItem([FromBody] ReturnDeliveryItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _deliveryService.ReturnDeliveryItemAsync(dto);
            if (!result)
                return NotFound("Teslimat kalemi bulunamadı.");

            return Ok("İade işlemi tamamlandı.");
        }

        // Kullanıcının kendi teslimatları
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetByMember(int memberId)
        {
            var requests = await _deliveryService.GetRequestsByMemberAsync(memberId);

            if (requests == null || !requests.Any())
                return Ok(new List<object>());

            return Ok(requests);
        }

    }
}
