using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTOs.DeliveryRequestDTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireLogin]
    public class ReturnsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ReturnsController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return View(new List<DeliveryRequestDto>());
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var baseUrl = ApiBaseUrl.TrimEnd('/');
                var url = $"{baseUrl}/api/DeliveryRequest/by-status?status={DeliveryStatus.Delivered}";

                var list = await client.GetFromJsonAsync<List<DeliveryRequestDto>>(url);
                return View(list ?? new List<DeliveryRequestDto>());
            }
            catch
            {
                TempData["Error"] = "İade ekranı verileri getirilemedi. API çalışıyor mu kontrol et.";
                return View(new List<DeliveryRequestDto>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var baseUrl = ApiBaseUrl.TrimEnd('/');
                var url = $"{baseUrl}/api/DeliveryRequest/{id}";

                var dto = await client.GetFromJsonAsync<DeliveryRequestDto>(url);
                if (dto == null)
                    return NotFound();

                // Delivered ama kalem yoksa (veya hepsi Id <= 0 ise) iade ekranını açma
                if (dto.Status == DeliveryStatus.Delivered &&
                    (dto.Items == null || !dto.Items.Any() || dto.Items.All(x => x.Id <= 0)))
                {
                    TempData["Error"] = "Bu teslimatta iade edilebilir kalem bulunmuyor. (Kalemler oluşturulmamış olabilir.)";
                    return RedirectToAction(nameof(Index));
                }

                return View(dto);
            }
            catch
            {
                TempData["Error"] = "Detay getirilemedi. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnItem(int requestId, int deliveryRequestItemId, bool isDamaged, string? note)
        {
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }

            if (deliveryRequestItemId <= 0)
            {
                TempData["Error"] = "Geçersiz kalem.";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var baseUrl = ApiBaseUrl.TrimEnd('/');

                var dto = new ReturnDeliveryItemDto
                {
                    DeliveryRequestItemId = deliveryRequestItemId,
                    IsDamaged = isDamaged,
                    Note = note
                };

                var res = await client.PostAsJsonAsync($"{baseUrl}/api/DeliveryRequest/return-item", dto);

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = string.IsNullOrWhiteSpace(msg) ? "İade işlemi başarısız." : msg;
                    return RedirectToAction(nameof(Details), new { id = requestId });
                }

                TempData["Success"] = "İade işlendi.";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }
            catch
            {
                TempData["Error"] = "İade işlemi başarısız. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Details), new { id = requestId });
            }
        }
    }
}
