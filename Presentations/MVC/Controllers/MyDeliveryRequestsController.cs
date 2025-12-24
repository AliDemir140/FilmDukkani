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

        public MyDeliveryRequestsController(DeliveryRequestServiceManager deliveryService)
        {
            _deliveryService = deliveryService;
        }

        public async Task<IActionResult> Index()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            var userId = HttpContext.Session.GetString(SessionKeys.UserId);
            var role = HttpContext.Session.GetString(SessionKeys.Role);

            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var list = await _deliveryService.GetRequestsByMemberAsync(memberId.Value);
            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var dto = await _deliveryService.GetRequestDetailAsync(id);
            if (dto == null)
                return NotFound();

            // Güvenlik: başkasının request’ini açamasın
            if (dto.MemberId != memberId.Value)
                return Forbid();

            return View(dto);
        }

    }
}
