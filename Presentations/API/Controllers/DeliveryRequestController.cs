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

        // POST api/DeliveryRequest/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDeliveryRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newId = await _deliveryService.CreateDeliveryRequestAsync(dto);

            return Ok(new
            {
                Message = "Teslimat isteği oluşturuldu.",
                RequestId = newId
            });
        }

        // POST api/DeliveryRequest/prepare-tomorrow
        [HttpPost("prepare-tomorrow")]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return Ok("Yarının teslimatları hazırlandı.");
        }

        // GET api/DeliveryRequest/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var request = await _deliveryService.GetRequestDetailAsync(id);
            if (request == null)
                return NotFound("Teslimat isteği bulunamadı.");

            return Ok(request);
        }

        // PUT api/DeliveryRequest/{id}/cancel
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var result = await _deliveryService.CancelRequestAsync(id);
            if (!result)
                return NotFound("Teslimat isteği bulunamadı.");

            return Ok("Teslimat isteği iptal edildi.");
        }

        // PUT api/DeliveryRequest/{id}/mark-delivered
        [HttpPut("{id}/mark-delivered")]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            var result = await _deliveryService.MarkDeliveredAsync(id);
            if (!result)
                return NotFound("Teslimat isteği bulunamadı.");

            return Ok("Teslimat isteği tamamlandı olarak işaretlendi.");
        }
    }
}
