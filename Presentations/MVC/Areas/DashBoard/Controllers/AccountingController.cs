using System.Net.Http.Headers;
using System.Text.Json;
using Application.Constants;
using Application.DTOs.AccountingDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Areas.DashBoard.Models;
using MVC.Constants;
using MVC.Filters;

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
                return View(new AccountingDashboardViewModel
                {
                    StartDate = DateTime.Today.AddDays(-30).Date,
                    EndDate = DateTime.Today.Date
                });
            }

            var s = (startDate ?? DateTime.Today.AddDays(-30)).Date;
            var e = (endDate ?? DateTime.Today).Date;

            if (s > e)
            {
                TempData["Error"] = "Başlangıç tarihi bitiş tarihinden büyük olamaz.";
                return View(new AccountingDashboardViewModel { StartDate = s, EndDate = e });
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

                model.ProfitLoss = await GetAsync<ProfitLossSummaryDto>(
                    client, $"{ApiBaseUrl}/api/accounting/profit-loss?{qs}");

                model.MemberReport = await GetAsync<List<MemberProfitReportDto>>(
                    client, $"{ApiBaseUrl}/api/accounting/member-report?{qs}") ?? new();

                model.MovieReport = await GetAsync<List<MovieProfitReportDto>>(
                    client, $"{ApiBaseUrl}/api/accounting/movie-report?{qs}") ?? new();

                model.CategoryReport = await GetAsync<List<CategoryProfitReportDto>>(
                    client, $"{ApiBaseUrl}/api/accounting/category-report?{qs}") ?? new();
            }
            catch
            {
                TempData["Error"] = "Muhasebe raporları çekilemedi.";
            }

            return View(model);
        }

        private static async Task<T?> GetAsync<T>(HttpClient client, string url)
        {
            using var res = await client.GetAsync(url);
            if (!res.IsSuccessStatusCode)
                return default;

            var json = await res.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
