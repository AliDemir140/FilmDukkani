using System.Net;
using System.Net.Http.Json;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;

namespace MVC.Controllers
{
    [RequireLogin]
    public class MyDeliveryRequestsController : Controller
    {
        private readonly DeliveryRequestServiceManager _deliveryService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MyDeliveryRequestsController(
            DeliveryRequestServiceManager deliveryService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _deliveryService = deliveryService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        public async Task<IActionResult> Index()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var list = await _deliveryService.GetRequestsByMemberAsync(memberId.Value);
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestCancel(int requestId, string reason)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (requestId <= 0)
            {
                TempData["Error"] = "Geçersiz sipariş.";
                return RedirectToAction(nameof(Index));
            }

            reason = (reason ?? "").Trim();
            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["Error"] = "İptal sebebi zorunludur.";
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

                var body = new { Reason = reason };

                var res = await client.PutAsJsonAsync(
                    $"{ApiBaseUrl}/api/DeliveryRequest/{requestId}/request-cancel?memberId={memberId.Value}",
                    body
                );

                if (res.StatusCode == HttpStatusCode.Conflict)
                {
                    TempData["Error"] = "Bu sipariş için zaten iptal talebi mevcut.";
                    return RedirectToAction(nameof(Index));
                }

                if (!res.IsSuccessStatusCode)
                {
                    TempData["Error"] = "İptal talebi oluşturulamadı. (Durum/tarih uygun değil)";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "İptal talebi alındı. Admin onayı bekleniyor.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "İptal talebi oluşturulamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var dto = await _deliveryService.GetRequestDetailAsync(id);
            if (dto == null)
                return NotFound();

            if (dto.MemberId != memberId.Value)
                return Forbid();

            return View(dto);
        }
    }
}
