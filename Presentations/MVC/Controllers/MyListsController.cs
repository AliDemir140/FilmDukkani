using System.Net;
using System.Net.Http.Json;
using Application.DTOs.MemberMovieListDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;
using MVC.Extensions;
using MVC.Models;

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

        [HttpGet]
        public IActionResult Create(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new CreateMemberMovieListDto());
        }

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

                bool locked = false;
                try
                {
                    var lockRes = await client.GetFromJsonAsync<Dictionary<string, bool>>(
                        $"{ApiBaseUrl}/api/MemberMovieList/is-locked?listId={id}"
                    );
                    locked = lockRes != null && lockRes.ContainsKey("locked") && lockRes["locked"];
                }
                catch { /* sessiz geç */ }

                ViewBag.ListName = selectedList.Name;
                ViewBag.ListId = selectedList.Id;
                ViewBag.IsLocked = locked;

                return View(items);
            }
            catch
            {
                TempData["Error"] = "Liste detayları alınamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Index));
            }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveItem(int itemId, int listId, string direction)
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

            direction = (direction ?? "").Trim().ToLower();
            if (direction != "up" && direction != "down")
            {
                TempData["Error"] = "Geçersiz yön.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var res = await client.PostAsync(
                    $"{ApiBaseUrl}/api/MemberMovieList/move-item?listId={listId}&itemId={itemId}&direction={direction}",
                    content: null
                );

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Öncelik güncellenemedi.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                TempData["Success"] = "Öncelik güncellendi.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
            catch
            {
                TempData["Error"] = "Öncelik güncellenemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddListToCart(int listId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (listId <= 0)
            {
                TempData["Error"] = "Geçersiz liste.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var lists = await client.GetFromJsonAsync<List<MemberMovieListDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/lists-by-member?memberId={memberId.Value}"
                ) ?? new List<MemberMovieListDto>();

                if (!lists.Any(x => x.Id == listId))
                    return Forbid();

                var items = await client.GetFromJsonAsync<List<MemberMovieListItemDto>>(
                    $"{ApiBaseUrl}/api/MemberMovieList/list-items-all?listId={listId}"
                ) ?? new List<MemberMovieListItemDto>();

                if (!items.Any())
                {
                    TempData["Error"] = "Bu listede film yok.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                var newCart = items
                    .GroupBy(x => x.MovieId)
                    .Select(g => g.First())
                    .Select(it => new CartItem
                    {
                        MovieId = it.MovieId,
                        MovieTitle = it.MovieTitle ?? "Film",
                        CoverImageUrl = "",
                        ReleaseYear = 0
                    })
                    .ToList();

                HttpContext.Session.SetObject(SessionKeys.Cart, newCart);

                TempData["Success"] = "Sepet seçili listeye göre güncellendi.";
                return RedirectToAction("Index", "Cart");
            }
            catch
            {
                TempData["Error"] = "Liste sepete eklenemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearList(int listId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (listId <= 0)
            {
                TempData["Error"] = "Geçersiz liste.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var res = await client.DeleteAsync($"{ApiBaseUrl}/api/MemberMovieList/clear-list-items?listId={listId}");

                if (res.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Error"] = "Bu liste aktif siparişe bağlı olduğu için kilitli. Listeyi boşaltamazsın. İptal talebi oluştur.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Liste boşaltılamadı.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                TempData["Success"] = "Liste boşaltıldı.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
            catch
            {
                TempData["Error"] = "Liste boşaltılamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAllNonOrdered()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var res = await client.PostAsync($"{ApiBaseUrl}/api/MemberMovieList/clear-all-nonordered?memberId={memberId.Value}", null);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Toplu temizlik yapılamadı.";
                    return RedirectToAction(nameof(Index));
                }

                var data = await res.Content.ReadFromJsonAsync<Dictionary<string, int>>();
                var clearedCount = data != null && data.ContainsKey("clearedCount") ? data["clearedCount"] : 0;

                TempData["Success"] = $"Toplu temizlik tamamlandı. Boşaltılan liste sayısı: {clearedCount}";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Toplu temizlik yapılamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rename(int listId, string name)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            name = (name ?? "").Trim();
            if (listId <= 0 || string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Liste adı zorunludur.";
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

                var dto = new UpdateMemberMovieListNameDto { Id = listId, Name = name };
                var res = await client.PutAsJsonAsync($"{ApiBaseUrl}/api/MemberMovieList/update-name", dto);

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Liste adı değiştirilemedi. (Aynı isim olabilir)";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                TempData["Success"] = "Liste adı güncellendi.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
            catch
            {
                TempData["Error"] = "Liste adı güncellenemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int listId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (listId <= 0)
            {
                TempData["Error"] = "Geçersiz liste.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var res = await client.DeleteAsync($"{ApiBaseUrl}/api/MemberMovieList/delete-list?listId={listId}");

                if (res.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Error"] = "Bu liste için aktif sipariş var. Liste silinemez.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Liste silinemedi.";
                    return RedirectToAction(nameof(Details), new { id = listId });
                }

                TempData["Success"] = "Liste silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Liste silinemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = listId });
            }
        }
    }
}
