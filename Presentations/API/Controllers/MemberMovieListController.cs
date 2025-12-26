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

        [HttpGet("check-minimum")]
        public async Task<IActionResult> CheckMinimum(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            var exists = await _service.ListExistsAsync(listId);
            if (!exists)
                return NotFound("Liste bulunamadı.");

            bool hasMinimum = await _service.HasMinimumItemsAsync(listId, 5);
            int currentCount = await _service.GetItemCountAsync(listId);

            return Ok(new
            {
                hasMinimum,
                minimumRequired = 5,
                currentCount
            });
        }

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

        // ✅ Liste adını değiştir
        // PUT: api/MemberMovieList/update-name
        [HttpPut("update-name")]
        public async Task<IActionResult> UpdateName([FromBody] UpdateMemberMovieListNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Id <= 0)
                return BadRequest("Id zorunludur.");

            dto.Name = (dto.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Liste adı zorunludur.");

            var ok = await _service.UpdateListNameAsync(dto);
            if (!ok)
                return BadRequest("Liste adı güncellenemedi. (Aynı isim kullanılmış olabilir)");

            return Ok("Liste adı güncellendi.");
        }

        // ✅ Tek listeyi boşalt (liste kalsın, itemlar silinsin)
        // DELETE: api/MemberMovieList/clear-list-items?listId=5
        [HttpDelete("clear-list-items")]
        public async Task<IActionResult> ClearListItems(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            var ok = await _service.ClearListItemsAsync(listId);
            if (!ok)
                return BadRequest("Liste boşaltılamadı.");

            return Ok("Liste boşaltıldı.");
        }

        // ✅ Listeyi sil (sadece aktif sipariş yoksa)
        // DELETE: api/MemberMovieList/delete-list?listId=5
        [HttpDelete("delete-list")]
        public async Task<IActionResult> DeleteList(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            var result = await _service.DeleteListAsync(listId);

            if (result == -1)
                return Conflict("Bu liste için aktif sipariş var. Liste silinemez.");

            if (result == 0)
                return NotFound("Liste bulunamadı.");

            return Ok("Liste silindi.");
        }

        // ✅ Toplu temizlik: siparişe girmemiş tüm listeleri boşalt
        // POST: api/MemberMovieList/clear-all-nonordered?memberId=1
        [HttpPost("clear-all-nonordered")]
        public async Task<IActionResult> ClearAllNonOrdered(int memberId)
        {
            if (memberId <= 0)
                return BadRequest("memberId zorunludur.");

            var cleared = await _service.ClearAllNonOrderedListsAsync(memberId);
            return Ok(new { clearedCount = cleared });
        }
    }
}
