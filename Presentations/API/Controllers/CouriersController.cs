using Application.DTOs.CourierDTOs;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouriersController : ControllerBase
    {
        private readonly CourierServiceManager _courierService;
        private readonly DeliveryRequestServiceManager _deliveryService;

        public CouriersController(CourierServiceManager courierService, DeliveryRequestServiceManager deliveryService)
        {
            _courierService = courierService;
            _deliveryService = deliveryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool onlyActive = false)
        {
            var list = await _courierService.GetAllAsync(onlyActive);
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _courierService.CreateAsync(dto);
            return Ok(new { id });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCourierDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _courierService.UpdateAsync(dto);
            if (!ok) return NotFound();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _courierService.DeleteAsync(id);
            if (!ok) return NotFound();

            return Ok();
        }

        [HttpGet("{courierId}/deliveries")]
        public async Task<IActionResult> GetDeliveries(
    int courierId,
    [FromQuery] DateTime date,
    [FromQuery] DeliveryStatus? status)
        {
            if (courierId <= 0) return BadRequest("courierId zorunludur.");
            if (date == default) date = DateTime.Today;

            var list = await _deliveryService.GetCourierDeliveriesAsync(courierId, date, status);
            return Ok(list);
        }

    }
}
