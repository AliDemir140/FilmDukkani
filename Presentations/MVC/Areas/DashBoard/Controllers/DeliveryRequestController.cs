using Application.DTOs.DeliveryRequestDTOs;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    public class DeliveryRequestController : Controller
    {
        private readonly DeliveryRequestServiceManager _deliveryService;

        public DeliveryRequestController(DeliveryRequestServiceManager deliveryService)
        {
            _deliveryService = deliveryService;
        }

        // /DashBoard/DeliveryRequest?status=Prepared
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

        // /DashBoard/DeliveryRequest/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var dto = await _deliveryService.GetRequestDetailAsync(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        // Yarını hazırla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return RedirectToAction(nameof(Index));
        }

        // İptal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _deliveryService.CancelRequestAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        // Teslim edildi
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDelivered(int id)
        {
            await _deliveryService.MarkDeliveredAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        // İade al
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnItem(ReturnDeliveryItemDto dto, int requestId)
        {
            await _deliveryService.ReturnDeliveryItemAsync(dto);
            return RedirectToAction(nameof(Details), new { id = requestId });
        }
    }
}
