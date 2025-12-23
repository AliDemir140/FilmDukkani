using System.Net.Http;
using System.Net.Http.Json;
using Application.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // LOGIN SAYFASI
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            var response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/api/auth/login",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                return View(model);
            }

            var loginResponse = await response.Content
                .ReadFromJsonAsync<LoginResponseDTO>();

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                ModelState.AddModelError("", "Giriş sırasında hata oluştu.");
                return View(model);
            }

            // JWT TOKEN SESSION'A YAZ
            HttpContext.Session.SetString("JWT", loginResponse.Token);
            HttpContext.Session.SetString("UserName", loginResponse.UserName);

            return RedirectToAction("Index", "Home");
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWT");
            HttpContext.Session.Remove("UserName");

            return RedirectToAction("Index", "Home");
        }
    }
}
