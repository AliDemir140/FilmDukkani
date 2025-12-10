using Application.DTOs.ShelfDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfController : ControllerBase
    {
        private readonly ShelfServiceManager _shelfService;

        public ShelfController(ShelfServiceManager shelfService)
        {
            _shelfService = shelfService;
        }

        // GET api/Shelf/shelves
        [HttpGet("shelves")]
        public async Task<IActionResult> GetShelves()
        {
            var shelves = await _shelfService.GetShelvesAsync();
            return Ok(shelves);
        }

        // GET api/Shelf/get-shelf?id=1
        [HttpGet("get-shelf")]
        public async Task<IActionResult> GetShelf(int id)
        {
            var shelf = await _shelfService.GetShelfAsync(id);
            if (shelf == null)
                return NotFound("Raf bulunamadı.");

            return Ok(shelf);
        }

        // POST api/Shelf/add-shelf
        [HttpPost("add-shelf")]
        public async Task<IActionResult> AddShelf([FromBody] CreateShelfDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _shelfService.AddShelfAsync(dto);
            return Ok("Raf eklendi.");
        }

        // PUT api/Shelf/update-shelf
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

        // DELETE api/Shelf/delete-shelf?id=1
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
