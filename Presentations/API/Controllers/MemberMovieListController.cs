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
    }
}
