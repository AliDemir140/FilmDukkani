using Application.Constants;
using Application.DTOs.ShelfDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin, RoleNames.Warehouse)]
    public class ShelfController : Controller
    {
        private readonly ShelfServiceManager _shelfService;

        public ShelfController(ShelfServiceManager shelfService)
        {
            _shelfService = shelfService;
        }

        // GET: /DashBoard/Shelf
        public async Task<IActionResult> Index()
        {
            var shelves = await _shelfService.GetShelvesAsync();
            return View(shelves);
        }

        // GET: /DashBoard/Shelf/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DashBoard/Shelf/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateShelfDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _shelfService.AddShelfAsync(dto);
            TempData["Success"] = "Raf eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Shelf/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _shelfService.GetShelfAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Shelf/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateShelfDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _shelfService.UpdateShelfAsync(dto);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Raf güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Shelf/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _shelfService.GetShelfAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Shelf/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _shelfService.DeleteShelfAsync(id);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Raf silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
