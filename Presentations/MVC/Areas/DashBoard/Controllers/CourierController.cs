using System.Net.Http.Json;
using Application.DTOs.CourierDTOs;
using Application.DTOs.DeliveryRequestDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireAdmin]
    public class CourierController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public CourierController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        public async Task<IActionResult> Distribution(int? courierId, DateTime? date, string? status)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return BadRequest("ApiSettings:BaseUrl bulunamadı.");

            var client = _httpClientFactory.CreateClient();

            var couriers = await client.GetFromJsonAsync<List<CourierDto>>($"{ApiBaseUrl}/api/couriers?onlyActive=true")
                          ?? new List<CourierDto>();

            ViewBag.Couriers = couriers;
            ViewBag.SelectedCourierId = courierId;
            ViewBag.SelectedDate = (date ?? DateTime.Today).Date;
            ViewBag.SelectedStatus = status ?? "";

            if (courierId == null || courierId <= 0)
                return View(new List<DeliveryRequestDto>());

            var d = (date ?? DateTime.Today).Date.ToString("yyyy-MM-dd");

            var url = $"{ApiBaseUrl}/api/couriers/{courierId}/deliveries?date={d}";
            if (!string.IsNullOrWhiteSpace(status))
                url += $"&status={status}";

            var deliveries = await client.GetFromJsonAsync<List<DeliveryRequestDto>>(url)
                           ?? new List<DeliveryRequestDto>();

            return View(deliveries);
        }

    }
}
