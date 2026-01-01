using Application.DTOs.ShelfDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "WarehouseAccess")]
    public class ShelfController : ControllerBase
    {
        private readonly ShelfServiceManager _shelfService;

        public ShelfController(ShelfServiceManager shelfService)
        {
            _shelfService = shelfService;
        }

        [HttpGet("shelves")]
        public async Task<IActionResult> GetShelves()
        {
            var shelves = await _shelfService.GetShelvesAsync();
            return Ok(shelves);
        }

        [HttpGet("get-shelf")]
        public async Task<IActionResult> GetShelf(int id)
        {
            var shelf = await _shelfService.GetShelfAsync(id);
            if (shelf == null)
                return NotFound("Raf bulunamadı.");

            return Ok(shelf);
        }

        [HttpPost("add-shelf")]
        public async Task<IActionResult> AddShelf([FromBody] CreateShelfDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _shelfService.AddShelfAsync(dto);
            return Ok("Raf eklendi.");
        }

        [HttpPut("update-shelf")]
        public async Task<IActionResult> UpdateShelf([FromBody] UpdateShelfDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _shelfService.UpdateShelfAsync(dto);
            if (!result)
                return NotFound("Raf bulunamadı.");

            return Ok("Raf güncellendi.");
        }

        [HttpDelete("delete-shelf")]
        public async Task<IActionResult> DeleteShelf(int id)
        {
            var result = await _shelfService.DeleteShelfAsync(id);
            if (!result)
                return NotFound("Raf bulunamadı.");

            return Ok("Raf silindi.");
        }
    }
}
