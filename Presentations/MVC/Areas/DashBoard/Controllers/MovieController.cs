// DOSYA YOLU:
// Presentations/MVC/Areas/DashBoard/Controllers/MovieController.cs

using Application.DTOs.MovieDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    public class MovieController : Controller
    {
        private readonly MovieServiceManager _movieService;

        public MovieController(MovieServiceManager movieService)
        {
            _movieService = movieService;
        }

        // GET: /DashBoard/Movie
        public async Task<IActionResult> Index()
        {
            var movies = await _movieService.GetMoviesAsync();
            return View(movies);
        }

        // GET: /DashBoard/Movie/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /DashBoard/Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMovieDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _movieService.AddMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError(nameof(dto.CategoryId), "Böyle bir kategori bulunamadı.");
                return View(dto);
            }

            TempData["Success"] = "Film eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Movie/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _movieService.GetMovie(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Movie/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMovieDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var ok = await _movieService.UpdateMovie(dto);
            if (!ok)
            {
                ModelState.AddModelError(nameof(dto.CategoryId), "Böyle bir kategori bulunamadı.");
                return View(dto);
            }

            TempData["Success"] = "Film güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DashBoard/Movie/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _movieService.GetMovie(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // POST: /DashBoard/Movie/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ok = await _movieService.DeleteMovie(id);

            if (!ok)
            {
                TempData["Error"] = "Film bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Film silindi.";
            return RedirectToAction(nameof(Index));
        }

    }
}
