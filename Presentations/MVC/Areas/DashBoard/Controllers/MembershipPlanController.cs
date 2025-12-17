using Application.DTOs.MembershipPlanDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    public class MembershipPlanController : Controller
    {
        private readonly MembershipPlanServiceManager _planService;

        public MembershipPlanController(MembershipPlanServiceManager planService)
        {
            _planService = planService;
        }

        // GET: /DashBoard/MembershipPlan
        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetPlansAsync();
            return View(plans);
        }

        // GET: /DashBoard/MembershipPlan/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DashBoard/MembershipPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMembershipPlanDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _planService.AddPlan(dto);

            TempData["Success"] = "Üyelik planı eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/MembershipPlan/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _planService.GetPlan(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/MembershipPlan/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMembershipPlanDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _planService.UpdatePlan(dto);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Üyelik planı güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/MembershipPlan/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _planService.GetPlan(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/MembershipPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _planService.DeletePlan(id);

            if (!ok)
            {
                TempData["Error"] = "Üyelik planı bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Üyelik planı silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
