using Application.DTOs.MemberMovieListDTOs;
using Application.DTOs.MovieDTOs;
using Application.DTOs.ReviewDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Constants;
using MVC.Extensions;
using MVC.Models;
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

        public async Task<IActionResult> Index(int? categoryId, string? q, string? showcase)
        {
            var categories = await _categoryService.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName", categoryId);

            showcase = NormalizeShowcase(showcase);

            List<MovieDto> movies;

            switch (showcase)
            {
                case "award":
                    movies = await _movieService.GetAwardWinnersAsync(200);
                    movies = ApplyCategoryAndQueryFilter(movies, categoryId, q);
                    break;

                case "top":
                    movies = await _movieService.GetTopRentedAsync(200);
                    movies = ApplyCategoryAndQueryFilter(movies, categoryId, q);
                    break;

                case "editors":
                    movies = await _movieService.GetEditorsChoiceAsync();
                    movies = ApplyCategoryAndQueryFilter(movies, categoryId, q);
                    break;

                case "new":
                    movies = await _movieService.GetNewReleasesAsync();
                    movies = ApplyCategoryAndQueryFilter(movies, categoryId, q);
                    break;

                default:
                    movies = await _movieService.SearchMoviesAsync(categoryId, q);
                    break;
            }

            ViewBag.Query = q ?? "";
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.Showcase = showcase;

            await LoadMemberListsForViewAsync();
            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieService.GetMovieDetailAsync(id);
            if (movie == null)
                return NotFound();

            await LoadMemberListsForViewAsync();
            await LoadReviewsForViewAsync(id);

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(int movieId, byte rating, string? comment)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            if (movieId <= 0 || rating < 1 || rating > 5)
            {
                TempData["Error"] = "Geçersiz puan.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var dto = new CreateReviewDto
                {
                    MovieId = movieId,
                    MemberId = memberId.Value,
                    Rating = rating,
                    Comment = comment
                };

                var res = await client.PostAsJsonAsync($"{apiBaseUrl}/api/Review/add-or-update", dto);

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg) ? "Yorum kaydedilemedi." : msg;
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                TempData["Success"] = "Yorum kaydedildi.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch
            {
                TempData["Error"] = "Yorum kaydedilemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int movieId, int reviewId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            if (movieId <= 0 || reviewId <= 0)
            {
                TempData["Error"] = "Geçersiz istek.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var res = await client.DeleteAsync($"{apiBaseUrl}/api/Review/{reviewId}?memberId={memberId.Value}");

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg) ? "Yorum silinemedi." : msg;
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                TempData["Success"] = "Yorum silindi.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch
            {
                TempData["Error"] = "Yorum silinemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
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
                    Priority = (priority.HasValue && priority.Value > 0) ? priority.Value : null
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

                await SyncSessionCartAfterAddAsync(movieId);

                TempData["Success"] = "Film listeye eklendi.";
                return RedirectToLocal(returnUrl);
            }
            catch
            {
                TempData["Error"] = "Film listeye eklenemedi. API çalışıyor mu kontrol et.";
                return RedirectToLocal(returnUrl);
            }
        }

        private async Task SyncSessionCartAfterAddAsync(int movieId)
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(SessionKeys.Cart);
            if (cart == null)
                return;

            if (cart.Any(x => x.MovieId == movieId))
                return;

            var movie = await _movieService.GetMovieDetailAsync(movieId);
            if (movie == null)
                return;

            cart.Add(new CartItem
            {
                MovieId = movieId,
                MovieTitle = movie.Title ?? "Film",
                CoverImageUrl = movie.CoverImageUrl ?? "",
                ReleaseYear = movie.ReleaseYear
            });

            HttpContext.Session.SetObject(SessionKeys.Cart, cart);
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

        private async Task LoadReviewsForViewAsync(int movieId)
        {
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ViewBag.Reviews = new List<ReviewDto>();
                return;
            }

            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);

            try
            {
                var client = _httpClientFactory.CreateClient();

                var url = $"{apiBaseUrl}/api/Review/movie/{movieId}";
                if (memberId.HasValue)
                    url += $"?currentMemberId={memberId.Value}";

                var reviews = await client.GetFromJsonAsync<List<ReviewDto>>(url);
                ViewBag.Reviews = reviews ?? new List<ReviewDto>();
            }
            catch
            {
                ViewBag.Reviews = new List<ReviewDto>();
            }
        }

        private static string NormalizeShowcase(string? showcase)
        {
            showcase = (showcase ?? "").Trim().ToLowerInvariant();

            return showcase switch
            {
                "award" => "award",
                "top" => "top",
                "editors" => "editors",
                "new" => "new",
                _ => ""
            };
        }

        private static List<MovieDto> ApplyCategoryAndQueryFilter(List<MovieDto> source, int? categoryId, string? q)
        {
            var query = (q ?? "").Trim();

            var list = source ?? new List<MovieDto>();

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                var cid = categoryId.Value;

                list = list.Where(m =>
                        (m.CategoryIds != null && m.CategoryIds.Contains(cid))
                        || m.CategoryId == cid
                    )
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                list = list.Where(m =>
                        (!string.IsNullOrWhiteSpace(m.Title) && m.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                        || (!string.IsNullOrWhiteSpace(m.OriginalTitle) && m.OriginalTitle.Contains(query, StringComparison.OrdinalIgnoreCase))
                        || (!string.IsNullOrWhiteSpace(m.Description) && m.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }

            return list;
        }
    }
}
