using Application.DTOs.MemberDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class MemberController : Controller
    {
        private readonly MemberServiceManager _memberService;
        private readonly MembershipPlanServiceManager _planService;

        public MemberController(MemberServiceManager memberService,
                                MembershipPlanServiceManager planService)
        {
            _memberService = memberService;
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetMembersAsync();
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Plans = await _planService.GetPlansAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberDto dto)
        {
            ViewBag.Plans = await _planService.GetPlansAsync();

            if (!ModelState.IsValid)
                return View(dto);

            await _memberService.AddMember(dto);

            TempData["Success"] = "Üye eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _memberService.GetMember(id); // UpdateMemberDto döndürsün
            if (dto == null)
                return NotFound();

            ViewBag.Plans = await _planService.GetPlansAsync();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plans = await _planService.GetPlansAsync();
                return View(dto);
            }

            var ok = await _memberService.UpdateMember(dto);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Üye güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _memberService.GetMember(id); // MemberDto döndürsün
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id) // kritik: Id
        {
            var ok = await _memberService.DeleteMember(Id);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Üye silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
