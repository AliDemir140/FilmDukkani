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

            var listId = await _service.CreateListAsync(dto);
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

        // diğer actionlar aynen kalabilir...
    }
}
