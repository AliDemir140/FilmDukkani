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

        // Üyeye ait listeleri getir
        [HttpGet("lists-by-member")]
        public async Task<IActionResult> GetListsByMember(int memberId)
        {
            var lists = await _service.GetListsByMemberAsync(memberId);
            return Ok(lists);
        }

        // Yeni liste oluştur
        [HttpPost("create-list")]
        public async Task<IActionResult> CreateList([FromBody] CreateMemberMovieListDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var listId = await _service.CreateListAsync(dto);
            return Ok(new { Message = "Liste oluşturuldu.", ListId = listId });
        }

        // Bir listenin film item'larını getir
        [HttpGet("list-items")]
        public async Task<IActionResult> GetListItems(int listId)
        {
            var items = await _service.GetListItemsAsync(listId);
            return Ok(items);
        }

        // Listeye film ekle
        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] CreateMemberMovieListItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddItemToListAsync(dto);

            if (!result)
                return BadRequest("Bu film zaten listede mevcut.");

            return Ok("Film listeye eklendi.");
        }

        // Liste adını güncelle
        [HttpPut("update-list-name")]
        public async Task<IActionResult> UpdateListName([FromBody] UpdateMemberMovieListNameDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateListNameAsync(dto);
            if (!result)
                return NotFound("Liste bulunamadı.");

            return Ok("Liste adı güncellendi.");
        }

        // Liste item'ını sil
        [HttpDelete("delete-item")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var result = await _service.DeleteItemAsync(id);
            if (!result)
                return NotFound("Liste elemanı bulunamadı.");

            return Ok("Liste elemanı silindi.");
        }

        // Liste item'ının önceliğini güncelle
        [HttpPut("update-item-priority")]
        public async Task<IActionResult> UpdateItemPriority([FromBody] UpdateMemberMovieListItemPriorityDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateItemPriorityAsync(dto);
            if (!result)
                return NotFound("Liste elemanı bulunamadı.");

            return Ok("Liste elemanı önceliği güncellendi.");
        }

        // Min 5 film kuralını kontrol et
        [HttpGet("check-minimum")]
        public async Task<IActionResult> CheckMinimum(int listId)
        {
            // Önce liste var mı
            var exists = await _service.ListExistsAsync(listId);
            if (!exists)
                return NotFound("Liste bulunamadı.");

            // Minimum 5 film var mı
            bool hasMinimum = await _service.HasMinimumItemsAsync(listId, 5);

            // Mevcut film sayısını da döndür
            int currentCount = await _service.GetItemCountAsync(listId);

            return Ok(new
            {
                hasMinimum = hasMinimum,
                minimumRequired = 5,
                currentCount = currentCount
            });
        }
    }
}
