using Application.DTOs.PurchaseRequestDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using System.Net.Http.Headers;
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var url = $"{apiBaseUrl.TrimEnd('/')}/api/PurchaseRequest/my?memberId={memberId.Value}";
                var res = await client.GetAsync(url);

                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = string.IsNullOrWhiteSpace(body)
                        ? $"Film istekleri getirilemedi. (HTTP {(int)res.StatusCode})"
                        : body;

                    return View(new List<PurchaseRequestDto>());
                }

                var list = await res.Content.ReadFromJsonAsync<List<PurchaseRequestDto>>();
                return View(list ?? new List<PurchaseRequestDto>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Film istekleri getirilemedi: " + ex.Message;
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var dto = new CreatePurchaseRequestDto
                {
                    MemberId = memberId.Value,
                    MovieId = movieId,
                    Quantity = quantity,
                    Note = note
                };

                var url = $"{apiBaseUrl.TrimEnd('/')}/api/PurchaseRequest/create";
                var res = await client.PostAsJsonAsync(url, dto);

                var body = await res.Content.ReadAsStringAsync();

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = string.IsNullOrWhiteSpace(body)
                        ? $"Talep oluşturulamadı. (HTTP {(int)res.StatusCode})"
                        : body;

                    return RedirectToLocal(returnUrl);
                }

                TempData["Success"] = "Film isteği oluşturuldu. Admin onayına düştü.";
                return RedirectToAction(nameof(My));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Talep oluşturulamadı: " + ex.Message;
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
