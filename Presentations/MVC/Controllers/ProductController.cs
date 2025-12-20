using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly MovieServiceManager _movieService;
        private readonly CategoryServiceManager _categoryService;

        public ProductController(MovieServiceManager movieService, CategoryServiceManager categoryService)
        {
            _movieService = movieService;
            _categoryService = categoryService;
        }

        // /Product?categoryId=1&q=matrix
        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", categoryId);

            var movies = await _movieService.GetMoviesAsync();

            if (categoryId.HasValue && categoryId.Value > 0)
                movies = movies.Where(m => m.CategoryId == categoryId.Value).ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var keyword = q.Trim().ToLower();
                movies = movies
                    .Where(m =>
                        (!string.IsNullOrWhiteSpace(m.Title) && m.Title.ToLower().Contains(keyword)) ||
                        (!string.IsNullOrWhiteSpace(m.OriginalTitle) && m.OriginalTitle.ToLower().Contains(keyword)))
                    .ToList();
            }

            ViewBag.Query = q ?? "";
            ViewBag.SelectedCategoryId = categoryId;

            return View(movies);
        }

        // /Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieDetailAsync(id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }
    }
}
