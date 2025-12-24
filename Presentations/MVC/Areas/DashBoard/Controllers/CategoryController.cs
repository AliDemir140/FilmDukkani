using Application.DTOs.CategoryDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class CategoryController : Controller
    {
        private readonly CategoryServiceManager _categoryService;

        public CategoryController(CategoryServiceManager categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: /DashBoard/Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }

        // GET: /DashBoard/Category/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DashBoard/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            await _categoryService.AddCategory(dto);

            TempData["Success"] = "Kategori eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Category/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _categoryService.GetCategory(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Category/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateCategoryDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _categoryService.UpdateCategory(dto);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Kategori güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Category/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _categoryService.GetCategory(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Category/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UpdateCategoryDto dto)
        {
            // Senin servicende DeleteCategory(UpdateCategoryDto dto) var.
            await _categoryService.DeleteCategory(dto);

            TempData["Success"] = "Kategori silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
