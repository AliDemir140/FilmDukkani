using Application.Constants;
using Application.DTOs.MemberMovieListDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin, RoleNames.Warehouse, RoleNames.Purchasing)]
    public class MemberMovieListController : Controller
    {
        private readonly MemberMovieListServiceManager _listService;
        private readonly MemberServiceManager _memberService;

        public MemberMovieListController(MemberMovieListServiceManager listService, MemberServiceManager memberService)
        {
            _listService = listService;
            _memberService = memberService;
        }

        private async Task LoadMembersAsync(int? selectedMemberId = null)
        {
            var members = await _memberService.GetMembersAsync();
            ViewBag.Members = new SelectList(members, "Id", "FullName", selectedMemberId);
        }

        // /DashBoard/MemberMovieList?memberId=1
        public async Task<IActionResult> Index(int? memberId)
        {
            await LoadMembersAsync(memberId);

            ViewBag.SelectedMemberId = memberId; // her zaman set

            if (memberId == null || memberId <= 0)
                return View(new List<MemberMovieListDto>());

            var lists = await _listService.GetListsByMemberAsync(memberId.Value);
            return View(lists);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? memberId)
        {
            await LoadMembersAsync(memberId);

            var dto = new CreateMemberMovieListDto
            {
                MemberId = memberId ?? 0
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberMovieListDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadMembersAsync(dto.MemberId);
                return View(dto);
            }

            await _listService.CreateListAsync(dto);
            return RedirectToAction(nameof(Index), new { memberId = dto.MemberId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditName(UpdateMemberMovieListNameDto dto, int memberId)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index), new { memberId });

            await _listService.UpdateListNameAsync(dto);
            return RedirectToAction(nameof(Index), new { memberId });
        }

        // Liste içeriği
        public async Task<IActionResult> Items(int id, int memberId)
        {
            ViewBag.MemberId = memberId;
            ViewBag.ListId = id;

            var items = await _listService.GetListItemsAsync(id);
            ViewBag.ItemCount = items.Count;

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePriority(UpdateMemberMovieListItemPriorityDto dto, int memberId, int listId)
        {
            await _listService.UpdateItemPriorityAsync(dto);
            return RedirectToAction(nameof(Items), new { id = listId, memberId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int itemId, int memberId, int listId)
        {
            await _listService.DeleteItemAsync(itemId);
            return RedirectToAction(nameof(Items), new { id = listId, memberId });
        }

        public class ReorderRequest
        {
            public int ListId { get; set; }
            public List<int> OrderedItemIds { get; set; } = new();
        }

        [HttpPost]
        public async Task<IActionResult> Reorder([FromBody] ReorderRequest req)
        {
            if (req == null || req.ListId <= 0 || req.OrderedItemIds == null || req.OrderedItemIds.Count == 0)
                return BadRequest();

            await _listService.ReorderItemsAsync(req.ListId, req.OrderedItemIds);
            return Ok();
        }

    }
}
