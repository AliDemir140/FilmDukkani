using System.Net.Http.Json;
using Application.DTOs.CourierDTOs;
using Application.DTOs.DeliveryRequestDTOs;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class DeliveryRequestController : Controller
    {
        private readonly DeliveryRequestServiceManager _deliveryService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DeliveryRequestController(
            DeliveryRequestServiceManager deliveryService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _deliveryService = deliveryService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        public async Task<IActionResult> Index(string? status)
        {
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<DeliveryStatus>(status, true, out var st))
            {
                var filtered = await _deliveryService.GetRequestsByStatusAsync(st);
                ViewBag.SelectedStatus = st.ToString();
                return View(filtered);
            }

            var all = await _deliveryService.GetAllRequestsAsync();
            ViewBag.SelectedStatus = "";
            return View(all);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dto = await _deliveryService.GetRequestDetailAsync(id);
            if (dto == null)
                return NotFound();

            // Kurye listesini Application'dan değil API'den al
            var couriers = await GetActiveCouriersSelectListFromApiAsync();

            if (dto.CourierId.HasValue)
            {
                foreach (var c in couriers)
                {
                    c.Selected = c.Value == dto.CourierId.Value.ToString();
                }
            }

            ViewBag.Couriers = couriers;
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _deliveryService.CancelRequestAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(int id)
        {
            await _deliveryService.ApproveCancelAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancel(int id)
        {
            await _deliveryService.RejectCancelAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkShipped(int id)
        {
            await _deliveryService.MarkShippedAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            await _deliveryService.MarkDeliveredAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            await _deliveryService.MarkCompletedAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnItem(ReturnDeliveryItemDto dto, int requestId)
        {
            await _deliveryService.ReturnDeliveryItemAsync(dto);
            return RedirectToAction(nameof(Details), new { id = requestId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignCourier(int id, int courierId)
        {
            var result = await _deliveryService.AssignCourierAsync(id, courierId);

            if (result == 0)
                TempData["Error"] = "Sipariş bulunamadı.";
            else if (result == -1)
                TempData["Error"] = "Kurye bulunamadı veya pasif.";
            else if (result == -2)
                TempData["Error"] = "Kurye ataması sadece Hazırlandı veya Kuryede durumunda yapılabilir.";
            else
                TempData["Success"] = "Kurye atandı.";

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<List<SelectListItem>> GetActiveCouriersSelectListFromApiAsync()
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return new List<SelectListItem>();

            var client = _httpClientFactory.CreateClient();

            var couriers = await client.GetFromJsonAsync<List<CourierDto>>(
                $"{ApiBaseUrl}/api/couriers?onlyActive=true"
            ) ?? new List<CourierDto>();

            return couriers
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToList();
        }
    }
}
