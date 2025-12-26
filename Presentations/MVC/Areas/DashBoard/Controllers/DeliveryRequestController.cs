using Application.DTOs.DeliveryRequestDTOs;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class DeliveryRequestController : Controller
    {
        private readonly DeliveryRequestServiceManager _deliveryService;

        public DeliveryRequestController(DeliveryRequestServiceManager deliveryService)
        {
            _deliveryService = deliveryService;
        }

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

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrepareTomorrow()
        {
            await _deliveryService.PrepareTomorrowDeliveriesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ❌ Eski direkt iptal kalsın istersen:
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _deliveryService.CancelRequestAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Yeni: iptal talebini ONAYLA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(int id)
        {
            await _deliveryService.ApproveCancelAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Yeni: iptal talebini REDDET
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
    }
}
