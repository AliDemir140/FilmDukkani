using Application.DTOs.MovieDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    public class MovieController : Controller
    {
        private readonly MovieServiceManager _movieService;
        private readonly CategoryServiceManager _categoryService;

        public MovieController(MovieServiceManager movieService, CategoryServiceManager categoryService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
        }

        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", selectedId);
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetMoviesAsync();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View(new CreateMovieDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                return View(dto);
            }

            var ok = await _movieService.AddMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Kategori bulunamadı.");
                await LoadCategoriesAsync(dto.CategoryId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _movieService.GetMovie(id);
            if (dto == null) return NotFound();

            await LoadCategoriesAsync(dto.CategoryId);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                return View(dto);
            }

            var ok = await _movieService.UpdateMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Güncelleme yapılamadı. Film veya kategori bulunamadı.");
                await LoadCategoriesAsync(dto.CategoryId);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _movieService.GetMovieDetailAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _movieService.DeleteMovie(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
