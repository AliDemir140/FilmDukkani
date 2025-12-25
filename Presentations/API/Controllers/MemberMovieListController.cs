using Application.DTOs.MemberMovieListDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberMovieListController : ControllerBase
    {
        private readonly MemberMovieListServiceManager _service;

        public MemberMovieListController(MemberMovieListServiceManager service)
        {
            _service = service;
        }

        [HttpGet("lists-by-member")]
        public async Task<IActionResult> GetListsByMember(int memberId)
        {
            if (memberId <= 0)
                return BadRequest("memberId zorunludur.");

            var lists = await _service.GetListsByMemberAsync(memberId);
            return Ok(lists);
        }

        [HttpPost("create-list")]
        public async Task<IActionResult> CreateList([FromBody] CreateMemberMovieListDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.MemberId <= 0)
                return BadRequest("MemberId zorunludur.");

            dto.Name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Liste adı zorunludur.");

            var listId = await _service.CreateListAsync(dto);

            if (listId == -1)
                return Conflict("Bu liste adı zaten mevcut."); 

            if (listId == 0)
                return BadRequest("Liste oluşturulamadı.");

            return Ok(new { Message = "Liste oluşturuldu.", ListId = listId });
        }


        [HttpGet("list-items")]
        public async Task<IActionResult> GetListItems(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            var items = await _service.GetListItemsAsync(listId);
            return Ok(items);
        }

        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] CreateMemberMovieListItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.MemberMovieListId <= 0)
                return BadRequest("MemberMovieListId zorunludur.");

            if (dto.MovieId <= 0)
                return BadRequest("MovieId zorunludur.");

            if (dto.Priority <= 0)
                dto.Priority = 1;

            var ok = await _service.AddItemToListAsync(dto);

            if (!ok)
                return BadRequest("Bu film zaten listede mevcut.");

            return Ok("Film listeye eklendi.");
        }

        [HttpDelete("delete-item")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            if (id <= 0)
                return BadRequest("id zorunludur.");

            var ok = await _service.DeleteItemAsync(id);

            if (!ok)
                return NotFound("Liste elemanı bulunamadı.");

            return Ok("Liste elemanı silindi.");
        }

        // ✅ Min 5 film kuralını kontrol et
        // GET: api/MemberMovieList/check-minimum?listId=1
        [HttpGet("check-minimum")]
        public async Task<IActionResult> CheckMinimum(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            // Önce liste var mı
            var exists = await _service.ListExistsAsync(listId);
            if (!exists)
                return NotFound("Liste bulunamadı.");

            // Minimum 5 film var mı
            bool hasMinimum = await _service.HasMinimumItemsAsync(listId, 5);

            // Mevcut film sayısı
            int currentCount = await _service.GetItemCountAsync(listId);

            return Ok(new
            {
                hasMinimum,
                minimumRequired = 5,
                currentCount
            });
        }

        // ✅ move-item (↑ ↓)
        // POST: api/MemberMovieList/move-item?listId=1&itemId=10&direction=up
        [HttpPost("move-item")]
        public async Task<IActionResult> MoveItem(int listId, int itemId, string direction)
        {
            if (listId <= 0 || itemId <= 0)
                return BadRequest("listId ve itemId zorunludur.");

            direction = (direction ?? "").Trim().ToLower();
            if (direction != "up" && direction != "down")
                return BadRequest("direction sadece 'up' veya 'down' olabilir.");

            var ok = await _service.MoveItemAsync(listId, itemId, direction);

            if (!ok)
                return BadRequest("Öncelik güncellenemedi.");

            return Ok("Öncelik güncellendi.");
        }
    }
}
