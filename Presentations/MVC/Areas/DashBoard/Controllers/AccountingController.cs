using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Constants;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;
using MVC.Areas.DashBoard.Models;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin, RoleNames.Accounting)]
    public class AccountingController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountingController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "ApiSettings:BaseUrl bulunamadı.";
                return View(new AccountingDashboardViewModel());
            }

            var s = (startDate ?? DateTime.Today.AddDays(-30)).Date;
            var e = (endDate ?? DateTime.Today).Date;

            if (s > e)
            {
                TempData["Error"] = "Başlangıç tarihi bitiş tarihinden büyük olamaz.";
                var bad = new AccountingDashboardViewModel { StartDate = s, EndDate = e };
                return View(bad);
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var model = new AccountingDashboardViewModel
            {
                StartDate = s,
                EndDate = e
            };

            try
            {
                var qs = $"startDate={s:yyyy-MM-dd}&endDate={e:yyyy-MM-dd}";

                model.ProfitLossJson = await GetJsonAsync(client, $"{ApiBaseUrl}/api/accounting/profit-loss?{qs}");
                model.MemberReportJson = await GetJsonAsync(client, $"{ApiBaseUrl}/api/accounting/member-report?{qs}");
                model.MovieReportJson = await GetJsonAsync(client, $"{ApiBaseUrl}/api/accounting/movie-report?{qs}");
                model.CategoryReportJson = await GetJsonAsync(client, $"{ApiBaseUrl}/api/accounting/category-report?{qs}");
            }
            catch
            {
                TempData["Error"] = "Muhasebe raporları çekilemedi.";
            }

            return View(model);
        }

        private static async Task<string> GetJsonAsync(HttpClient client, string url)
        {
            using var res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return string.Empty;

            return await res.Content.ReadAsStringAsync();
        }
    }
}
