using Application.DTOs.WarehouseDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly WarehouseServiceManager _warehouseService;

        public WarehouseController(WarehouseServiceManager warehouseService)
        {
            _warehouseService = warehouseService;
        }

        // GET: api/Warehouse/shelf-inventory?shelfId=1
        [HttpGet("shelf-inventory")]
        public async Task<IActionResult> GetShelfInventory(int shelfId)
        {
            var inventory = await _warehouseService.GetShelfInventoryAsync(shelfId);
            return Ok(inventory);
        }

        // PUT: api/Warehouse/move-copy-to-shelf
        [HttpPut("move-copy-to-shelf")]
        public async Task<IActionResult> MoveCopyToShelf([FromBody] MoveCopyToShelfDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _warehouseService.MoveCopyToShelfAsync(dto);
            if (!result)
                return NotFound("Film kopyası veya raf bulunamadı.");

            return Ok("Film kopyası rafa taşındı.");
        }

        // POST: api/Warehouse/process-return
        [HttpPost("process-return")]
        public async Task<IActionResult> ProcessReturn([FromBody] ProcessReturnDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _warehouseService.ProcessReturnAsync(dto);
            if (!result)
                return NotFound("Film kopyası bulunamadı.");

            return Ok("İade işlemi tamamlandı.");
        }
    }
}
