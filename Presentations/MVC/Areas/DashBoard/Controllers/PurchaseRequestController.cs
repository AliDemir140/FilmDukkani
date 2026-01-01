using Application.Constants;
using Application.DTOs.PurchaseRequestDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin, RoleNames.Purchasing)]
    public class PurchaseRequestController : Controller
    {
        private readonly PurchaseRequestServiceManager _purchaseService;

        public PurchaseRequestController(PurchaseRequestServiceManager purchaseService)
        {
            _purchaseService = purchaseService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _purchaseService.GetAllAsync();
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? decisionNote)
        {
            var ok = await _purchaseService.DecideAsync(new DecidePurchaseRequestDto
            {
                RequestId = id,
                Approved = true,
                AdminNote = decisionNote
            });

            TempData[ok ? "Success" : "Error"] = ok ? "Talep onaylandı." : "Talep onaylanamadı.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? decisionNote)
        {
            var ok = await _purchaseService.DecideAsync(new DecidePurchaseRequestDto
            {
                RequestId = id,
                Approved = false,
                AdminNote = decisionNote
            });

            TempData[ok ? "Success" : "Error"] = ok ? "Talep reddedildi." : "Talep reddedilemedi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
