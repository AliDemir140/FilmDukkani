using Application.DTOs.BillingDTOs;
using Application.DTOs.MemberDTOs;
using Application.DTOs.MembershipPlanDTOs;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVC.Constants;
using MVC.Filters;
using MVC.Models;
using System.Net.Http.Json;

namespace MVC.Controllers
{
    [RequireLogin]
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        [HttpGet]
        public async Task<IActionResult> Membership()
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var vm = new AccountMembershipViewModel();

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return View(vm);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var profile = await client.GetFromJsonAsync<MemberProfileDto>(
                    $"{ApiBaseUrl}/api/MemberProfile/profile?memberId={memberId.Value}"
                );

                vm.Profile = profile;

                var plans = await client.GetFromJsonAsync<List<MembershipPlanDto>>(
                    $"{ApiBaseUrl}/api/MembershipPlan/plans"
                ) ?? new List<MembershipPlanDto>();

                vm.Plans = plans.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.PlanName} ({p.Price}₺)"
                }).ToList();

                vm.SelectedPlanId = profile != null ? profile.MembershipPlanId : (plans.FirstOrDefault()?.Id ?? 0);

                if (profile != null)
                {
                    vm.StatusText = profile.Status.ToString();
                    vm.IsBlockedForCheckout = profile.Status == MemberStatus.PaymentDue || profile.Status == MemberStatus.Suspended;
                    vm.IsActive = profile.Status == MemberStatus.Active;
                }

                return View(vm);
            }
            catch
            {
                TempData["Error"] = "Üyelik bilgileri alınamadı. API çalışıyor mu kontrol et.";
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMembershipPlan(int selectedPlanId)
        {
            var memberId = HttpContext.Session.GetInt32(SessionKeys.MemberId);
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            if (selectedPlanId <= 0)
            {
                TempData["Error"] = "Plan seçimi zorunludur.";
                return RedirectToAction(nameof(Membership));
            }

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "API BaseUrl bulunamadı.";
                return RedirectToAction(nameof(Membership));
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                var dto = new ChargeMembershipDto
                {
                    MemberId = memberId.Value,
                    MembershipPlanId = selectedPlanId
                };

                var res = await client.PostAsJsonAsync($"{ApiBaseUrl}/api/Billing/charge-now", dto);

                if (!res.IsSuccessStatusCode)
                {
                    var msg = await TryReadMessageAsync(res);
                    TempData["Error"] = msg ?? "Ödeme başarısız.";
                    return RedirectToAction(nameof(Membership));
                }

                TempData["Success"] = "Ödeme başarılı. Üyeliğin aktifleştirildi.";
                return RedirectToAction(nameof(Membership));
            }
            catch
            {
                TempData["Error"] = "Ödeme yapılamadı. API çalışıyor mu kontrol et.";
                return RedirectToAction(nameof(Membership));
            }
        }

        private static async Task<string?> TryReadMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var obj = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                if (obj == null) return null;

                if (obj.TryGetValue("message", out var m1) && m1 != null)
                    return m1.ToString();

                if (obj.TryGetValue("Message", out var m2) && m2 != null)
                    return m2.ToString();

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
