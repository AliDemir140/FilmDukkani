using Application.DTOs.MovieCopyDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class MovieCopyController : Controller
    {
        private readonly MovieCopyServiceManager _movieCopyService;
        private readonly MovieServiceManager _movieService;
        private readonly ShelfServiceManager _shelfService;

        public MovieCopyController(
            MovieCopyServiceManager movieCopyService,
            MovieServiceManager movieService,
            ShelfServiceManager shelfService)
        {
            _movieCopyService = movieCopyService;
            _movieService = movieService;
            _shelfService = shelfService;
        }

        private async Task LoadDropdownsAsync()
        {
            var movies = await _movieService.GetMoviesAsync();
            ViewBag.Movies = movies.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Title
            }).ToList();

            var shelves = await _shelfService.GetShelvesAsync();
            ViewBag.Shelves = shelves.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
        }

        public async Task<IActionResult> Index()
        {
            var list = await _movieCopyService.GetMovieCopiesAsync();
            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View(new CreateMovieCopyDto { IsAvailable = true, IsDamaged = false });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieCopyDto dto)
        {
            await LoadDropdownsAsync();

            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _movieCopyService.AddMovieCopyAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Film bulunamadı veya bilgiler geçersiz.");
                return View(dto);
            }

            TempData["Success"] = "Film kopyası eklendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            await LoadDropdownsAsync();

            var dto = await _movieCopyService.GetMovieCopyAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMovieCopyDto dto)
        {
            await LoadDropdownsAsync();

            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _movieCopyService.UpdateMovieCopyAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Kopya veya film bulunamadı.");
                return View(dto);
            }

            TempData["Success"] = "Film kopyası güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _movieCopyService.GetMovieCopyAsync(id);
            if (dto == null) return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _movieCopyService.DeleteMovieCopyAsync(id);
            if (!ok)
            {
                TempData["Error"] = "Film kopyası bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Film kopyası silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
