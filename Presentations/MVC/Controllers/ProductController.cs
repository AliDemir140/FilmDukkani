using Application.DTOs.MemberMovieListDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Constants;
using System.Net;
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

        public async Task<IActionResult> Index(int? categoryId, string? q)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", categoryId);

            var movies = await _movieService.SearchMoviesAsync(categoryId, q);

            ViewBag.Query = q ?? "";
            ViewBag.SelectedCategoryId = categoryId;

            await LoadMemberListsForViewAsync();
            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieDetailAsync(id);
            if (movie == null)
                return NotFound();

            await LoadMemberListsForViewAsync();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToList(int movieId, int listId, int? priority, string? returnUrl = null)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (movieId <= 0 || listId <= 0)
            {
                TempData["Error"] = "Geçersiz istek.";
                return RedirectToLocal(returnUrl);
            }

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToLocal(returnUrl);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var dto = new CreateMemberMovieListItemDto
                {
                    MemberMovieListId = listId,
                    MovieId = movieId,
                    Priority = priority.HasValue && priority.Value > 0 ? priority.Value : 1
                };

                var res = await client.PostAsJsonAsync($"{apiBaseUrl}/api/MemberMovieList/add-item", dto);

                if (res.StatusCode == HttpStatusCode.Conflict)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg)
                        ? "Bu liste aktif siparişe bağlı olduğu için kilitlidir. Film eklenemez."
                        : msg;
                    return RedirectToLocal(returnUrl);
                }

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg)
                        ? "Film listeye eklenemedi."
                        : msg;
                    return RedirectToLocal(returnUrl);
                }

                TempData["Success"] = "Film listeye eklendi.";
                return RedirectToLocal(returnUrl);
            }
            catch
            {
                TempData["Error"] = "Film listeye eklenemedi. API çalışıyor mu kontrol et.";
                return RedirectToLocal(returnUrl);
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(Index));
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
