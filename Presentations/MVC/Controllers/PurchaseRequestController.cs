using Application.DTOs.PurchaseRequestDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using System.Net.Http.Json;

namespace MVC.Controllers
{
    public class PurchaseRequestController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public PurchaseRequestController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> My()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);

            if (!memberId.HasValue || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth");

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                TempData["Error"] = "ApiSettings:BaseUrl bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var url = $"{apiBaseUrl.TrimEnd('/')}/api/PurchaseRequest/my?memberId={memberId.Value}";
                var list = await client.GetFromJsonAsync<List<PurchaseRequestDto>>(url);

                return View(list ?? new List<PurchaseRequestDto>());
            }
            catch
            {
                TempData["Error"] = "Film istekleri getirilemedi. API çalışıyor mu kontrol et.";
                return View(new List<PurchaseRequestDto>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int movieId, int quantity, string? note, string? returnUrl = null)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);

            if (!memberId.HasValue || string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { returnUrl });

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                TempData["Error"] = "ApiSettings:BaseUrl bulunamadı.";
                return RedirectToLocal(returnUrl);
            }

            if (movieId <= 0 || quantity <= 0)
            {
                TempData["Error"] = "Geçersiz istek.";
                return RedirectToLocal(returnUrl);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var dto = new CreatePurchaseRequestDto
                {
                    MovieId = movieId,
                    Quantity = quantity,
                    Note = note
                };

                var res = await client.PostAsJsonAsync($"{apiBaseUrl.TrimEnd('/')}/api/PurchaseRequest/create", dto);

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg) ? "Talep oluşturulamadı." : msg;
                    return RedirectToLocal(returnUrl);
                }

                TempData["Success"] = "Film isteği oluşturuldu. Admin onayına düştü.";
                return RedirectToAction(nameof(My));
            }
            catch
            {
                TempData["Error"] = "Talep oluşturulamadı. API çalışıyor mu kontrol et.";
                return RedirectToLocal(returnUrl);
            }
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
