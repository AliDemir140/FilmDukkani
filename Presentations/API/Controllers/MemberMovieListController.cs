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
        // GET: api/MemberMovieList/lists-by-member?memberId=1
        [HttpGet("lists-by-member")]
        public async Task<IActionResult> GetListsByMember(int memberId)
        {
            if (memberId <= 0)
                return BadRequest("memberId zorunludur.");

            var lists = await _service.GetListsByMemberAsync(memberId);
            return Ok(lists);
        }

        // Yeni liste oluştur
        // POST: api/MemberMovieList/create-list
        [HttpPost("create-list")]
        public async Task<IActionResult> CreateList([FromBody] CreateMemberMovieListDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.MemberId <= 0)
                return BadRequest("MemberId zorunludur.");

            var listId = await _service.CreateListAsync(dto);
            return Ok(new { Message = "Liste oluşturuldu.", ListId = listId });
        }

        // Bir listenin item'larını getir
        // GET: api/MemberMovieList/list-items?listId=1
        [HttpGet("list-items")]
        public async Task<IActionResult> GetListItems(int listId)
        {
            if (listId <= 0)
                return BadRequest("listId zorunludur.");

            var items = await _service.GetListItemsAsync(listId);
            return Ok(items);
        }

        // Listeye film ekle
        // POST: api/MemberMovieList/add-item
        [HttpPost("add-item")]
        public async Task<IActionResult> AddItem([FromBody] CreateMemberMovieListItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.MemberMovieListId <= 0)
                return BadRequest("MemberMovieListId zorunludur.");

            if (dto.MovieId <= 0)
                return BadRequest("MovieId zorunludur.");

            // Priority UI'den gelmeyebilir, yine de >= 1 olsun
            if (dto.Priority <= 0)
                dto.Priority = 1;

            var ok = await _service.AddItemToListAsync(dto);

            if (!ok)
                return BadRequest("Bu film zaten listede mevcut.");

            return Ok("Film listeye eklendi.");
        }
    }
}
