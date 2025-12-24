using System.Net.Http.Json;
using Application.DTOs.UserDTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;

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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

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

            var loginResponse =
                await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

            if (loginResponse == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                ModelState.AddModelError("", "Giriş sırasında hata oluştu.");
                return View(model);
            }

            HttpContext.Session.SetString(SessionKeys.JwtToken, loginResponse.Token);
            HttpContext.Session.SetString(SessionKeys.UserName, loginResponse.UserName);
            HttpContext.Session.SetString(SessionKeys.Role, loginResponse.Role);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _httpClientFactory.CreateClient();
            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];

            var response = await client.PostAsJsonAsync(
                $"{apiBaseUrl}/api/auth/register",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Kayıt başarısız.");
                return View(model);
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
