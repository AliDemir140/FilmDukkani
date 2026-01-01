using Application.Constants;
using Application.DTOs.MovieDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin, RoleNames.Warehouse)]
    public class MovieController : Controller
    {
        private readonly MovieServiceManager _movieService;
        private readonly CategoryServiceManager _categoryService;
        private readonly ActorServiceManager _actorService;
        private readonly DirectorServiceManager _directorService;

        public MovieController(
            MovieServiceManager movieService,
            CategoryServiceManager categoryService,
            ActorServiceManager actorService,
            DirectorServiceManager directorService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _actorService = actorService;
            _directorService = directorService;
        }

        private async Task LoadCategoriesAsync(List<int>? selectedIds = null)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new MultiSelectList(categories, "Id", "CategoryName", selectedIds);
        }

        private async Task LoadActorsAsync(List<int>? selectedIds = null)
        {
            var actors = await _actorService.GetActorsForSelectAsync();
            ViewBag.Actors = new MultiSelectList(actors, "Id", "FullName", selectedIds);
        }

        private async Task LoadDirectorsAsync(List<int>? selectedIds = null)
        {
            var directors = await _directorService.GetDirectorsForSelectAsync();
            ViewBag.Directors = new MultiSelectList(directors, "Id", "FullName", selectedIds);
        }

        private async Task LoadLookupsAsync(List<int>? categoryIds = null, List<int>? actorIds = null, List<int>? directorIds = null)
        {
            await LoadCategoriesAsync(categoryIds);
            await LoadActorsAsync(actorIds);
            await LoadDirectorsAsync(directorIds);
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetMoviesAsync();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            return View(new CreateMovieDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(dto.CategoryIds, dto.ActorIds, dto.DirectorIds);
                return View(dto);
            }

            var ok = await _movieService.AddMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Kategori bulunamadı veya seçim geçersiz.");
                await LoadLookupsAsync(dto.CategoryIds, dto.ActorIds, dto.DirectorIds);
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _movieService.GetMovie(id);
            if (dto == null) return NotFound();

            await LoadLookupsAsync(dto.CategoryIds, dto.ActorIds, dto.DirectorIds);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMovieDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync(dto.CategoryIds, dto.ActorIds, dto.DirectorIds);
                return View(dto);
            }

            var ok = await _movieService.UpdateMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Güncelleme yapılamadı. Film veya kategori bulunamadı.");
                await LoadLookupsAsync(dto.CategoryIds, dto.ActorIds, dto.DirectorIds);
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
