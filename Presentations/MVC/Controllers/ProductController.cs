using Application.DTOs.MemberMovieListDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Constants;
using System.Net.Http.Json;

namespace MVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly MovieServiceManager _movieService;
        private readonly CategoryServiceManager _categoryService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ProductController(
            MovieServiceManager movieService,
            CategoryServiceManager categoryService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _movieService = movieService;
            _categoryService = categoryService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // /Product?categoryId=1&q=matrix
        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            // Kategoriler
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", categoryId);

            // Filmler
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

            // Login varsa listeleri çek (dropdown için)
            await LoadMemberListsForViewAsync();

            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieDetailAsync(id);
            if (movie == null)
                return NotFound();

            // Detay sayfasında da listeye eklemek istersen kullanırsın
            await LoadMemberListsForViewAsync();

            return View(movie);
        }

        private async Task LoadMemberListsForViewAsync()
        {
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);

            if (string.IsNullOrEmpty(token) || memberId == null)
            {
                ViewBag.MemberLists = new List<SelectListItem>();
                return;
            }

            try
            {
                var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
                var client = _httpClientFactory.CreateClient();

                var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                    $"{apiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId.Value}"
                );

                lists ??= new List<MemberMovieListDto>();

                ViewBag.MemberLists = lists
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList();
            }
            catch
            {
                ViewBag.MemberLists = new List<SelectListItem>();
            }
        }
    }
}
