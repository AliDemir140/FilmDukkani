using Application.DTOs.DeliveryRequestDTOs;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "WarehouseAccess")]
    public class DeliveryRequestController : ControllerBase
    {
        private readonly DeliveryRequestServiceManager _deliveryService;

        public DeliveryRequestController(DeliveryRequestServiceManager deliveryService)
        {
            _deliveryService = deliveryService;
        }

        [HttpGet("by-status")]
        public async Task<IActionResult> GetByStatus([FromQuery] DeliveryStatus status)
        {
            var list = await _deliveryService.GetRequestsByStatusAsync(status);
            return Ok(list ?? new List<DeliveryRequestDto>());
        }

        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newId = await _deliveryService.CreateDeliveryRequestAsync(dto);

            if (newId == 0)
                return BadRequest("Geçersiz teslimat tarihi. Teslimat en az 2 gün sonrası olmalı ve Pazar günü seçilemez.");

            if (newId == -1)
                return Conflict("Bu liste için zaten aktif bir sipariş var. (Bekliyor/Hazırlanıyor/Kuryede/Teslim Edildi)");

            return Ok(new { Message = "Teslimat isteği oluşturuldu.", RequestId = newId });
        }

        [HttpPost("prepare-tomorrow")]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return Ok("Yarının teslimatları hazırlandı.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var request = await _deliveryService.GetRequestDetailAsync(id);
            if (request == null)
                return NotFound("Teslimat isteği bulunamadı.");

            return Ok(request);
        }

        [AllowAnonymous]
        [HttpPost("{id}/cancel-request")]
        public async Task<IActionResult> CancelRequestByUser(int id, int memberId, [FromQuery] string? reason)
        {
            if (id <= 0 || memberId <= 0)
                return BadRequest("id ve memberId zorunludur.");

            var result = await _deliveryService.UserCancelRequestAsync(memberId, id, reason ?? "");

            if (result == 0) return NotFound("Teslimat isteği bulunamadı.");
            if (result == -1) return Forbid();
            if (result == -2) return BadRequest("Bu sipariş iptal talebi alamaz (Tamamlandı/İptal).");
            if (result == -3) return Conflict("Bu sipariş için zaten bekleyen bir iptal talebi var.");
            if (result == -4) return BadRequest("İptal için geç kaldın. (En geç 1 gün önce)");

            return Ok("İptal talebi oluşturuldu. Admin onayı bekleniyor.");
        }

        [HttpPost("{id}/cancel-decision")]
        public async Task<IActionResult> CancelDecision(int id, bool approve)
        {
            if (id <= 0)
                return BadRequest("id zorunludur.");

            var result = await _deliveryService.AdminDecideCancelAsync(id, approve);

            if (result == 0) return NotFound("Teslimat isteği bulunamadı.");
            if (result == -1) return BadRequest("İptal talebi yok veya zaten karar verilmiş.");

            return Ok(approve ? "İptal onaylandı." : "İptal reddedildi.");
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _deliveryService.CancelRequestAsync(id);
            if (!result)
                return BadRequest("Teslimat isteği iptal edilemedi.");

            return Ok("Teslimat isteği iptal edildi.");
        }

        [HttpPut("{id}/mark-delivered")]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var result = await _deliveryService.MarkDeliveredAsync(id);
            if (!result)
                return BadRequest("Teslimat isteği teslim edildi olarak işaretlenemedi.");

            return Ok("Teslimat teslim edildi ve süreç tamamlandı.");
        }

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

        [AllowAnonymous]
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetByMember(int memberId)
        {
            var requests = await _deliveryService.GetRequestsByMemberAsync(memberId);

            if (requests == null || !requests.Any())
                return Ok(new List<object>());

            return Ok(requests);
        }

        [HttpPut("{id}/assign-courier")]
        public async Task<IActionResult> AssignCourier(int id, [FromQuery] int courierId)
        {
            if (id <= 0 || courierId <= 0)
                return BadRequest("id ve courierId zorunludur.");

            var result = await _deliveryService.AssignCourierAsync(id, courierId);

            if (result == 0) return NotFound("Teslimat isteği bulunamadı.");
            if (result == -1) return BadRequest("Kurye bulunamadı veya pasif.");
            if (result == -2) return BadRequest("Kurye ataması sadece Hazırlandı veya Kuryede durumunda yapılabilir.");

            return Ok("Kurye atandı.");
        }

        [HttpPut("{id}/mark-shipped")]
        public async Task<IActionResult> MarkShipped(int id)
        {
            var result = await _deliveryService.MarkShippedAsync(id);
            if (!result)
                return BadRequest("Teslimat isteği kuryeye çıktı olarak işaretlenemedi.");

            return Ok("Teslimat kuryeye çıktı olarak işaretlendi.");
        }
    }
}
