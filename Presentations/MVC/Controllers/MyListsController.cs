using System.Net.Http.Json;
using Application.DTOs.MemberMovieListDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;

namespace MVC.Controllers
{
    [RequireLogin]
    public class MyListsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MyListsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        // GET: /MyLists
        public async Task<IActionResult> Index()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı. appsettings.json kontrol et.";
                return View(new List<MemberMovieListDto>());
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId.Value}"
                );

                return View(lists ?? new List<MemberMovieListDto>());
            }
            catch
            {
                TempData["Error"] = "Listeler alınamadı. API çalışıyor mu kontrol et.";
                return View(new List<MemberMovieListDto>());
            }
        }

        // GET: /MyLists/Create?returnUrl=/Cart/Checkout
        [HttpGet]
        public IActionResult Create(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CreateMemberMovieListDto());
        }

        // POST: /MyLists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberMovieListDto dto, string? returnUrl = null)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            dto.MemberId = memberId.Value;
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(dto);

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                ModelState.AddModelError("", "API BaseUrl bulunamadı. appsettings.json kontrol et.");
                return View(dto);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsJsonAsync(
                    $"{ApiBaseUrl}/api/MemberMovieList/create-list",
                    dto
                );

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("", "Liste oluşturulamadı.");
                    return View(dto);
                }

                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Liste oluşturulamadı. API çalışıyor mu kontrol et.");
                return View(dto);
            }
        }

        // GET: /MyLists/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return NotFound("API BaseUrl bulunamadı.");

            try
            {
                var client = _httpClientFactory.CreateClient();

                var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId.Value}"
                ) ?? new List<MemberMovieListDto>();

                var selectedList = lists.FirstOrDefault(x => x.Id == id);
                if (selectedList == null)
                    return Forbid();

                var items = await client.GetFromJsonAsync<List<MemberMovieListItemDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/list-items?listId={id}"
                ) ?? new List<MemberMovieListItemDto>();

                ViewBag.ListName = selectedList.Name;
                ViewBag.ListId = selectedList.Id;

                return View(items);
            }
            catch
            {
                TempData["Error"] = "Liste detayları alınamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /MyLists/DeleteItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItem(int itemId, int listId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (itemId <= 0 || listId <= 0)
            {
                TempData["Error"] = "Geçersiz istek.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                // API: /api/MemberMovieList/delete-item?id=123
                var res = await client.DeleteAsync($"{ApiBaseUrl}/api/MemberMovieList/delete-item?id={itemId}");

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Film listeden silinemedi.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                TempData["Success"] = "Film listeden silindi.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
            catch
            {
                TempData["Error"] = "Silme işlemi sırasında hata oluştu. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }
    }
}
