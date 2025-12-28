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

        public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
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

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError("", "ApiSettings:BaseUrl ayarı bulunamadı.");
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();

                using var response = await client.PostAsJsonAsync($"{apiBaseUrl}/api/auth/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    var apiMsg = await TryReadMessageAsync(response);
                    ModelState.AddModelError("", apiMsg ?? "Kullanıcı adı veya şifre hatalı.");
                    return View(model);
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDTO>();

                if (loginResponse == null || string.IsNullOrWhiteSpace(loginResponse.Token))
                {
                    ModelState.AddModelError("", "Giriş sırasında hata oluştu (token boş).");
                    return View(model);
                }

                HttpContext.Session.SetString(SessionKeys.JwtToken, loginResponse.Token);
                HttpContext.Session.SetString(SessionKeys.UserId, loginResponse.UserId ?? string.Empty);
                HttpContext.Session.SetString(SessionKeys.UserName, loginResponse.UserName ?? string.Empty);
                HttpContext.Session.SetString(SessionKeys.Role, loginResponse.Role ?? "User");

                using var memberRes = await client.GetAsync($"{apiBaseUrl}/api/members/by-user/{loginResponse.UserId}");

                if (!memberRes.IsSuccessStatusCode)
                {
                    HttpContext.Session.Clear();

                    if ((int)memberRes.StatusCode == 404)
                    {
                        ModelState.AddModelError("",
                            "Üye kaydı bulunamadı. Kayıt işlemi eksik kalmış olabilir.");
                        return View(model);
                    }

                    var apiMsg = await TryReadMessageAsync(memberRes);
                    ModelState.AddModelError("",
                        apiMsg ?? $"Member servisine erişilemedi. (HTTP {(int)memberRes.StatusCode})");
                    return View(model);
                }

                int memberId;
                try
                {
                    var read = await memberRes.Content.ReadFromJsonAsync<int?>();
                    memberId = read ?? 0;
                }
                catch
                {
                    memberId = 0;
                }

                if (memberId <= 0)
                {
                    HttpContext.Session.Clear();
                    ModelState.AddModelError("", "MemberId okunamadı. Lütfen tekrar deneyin.");
                    return View(model);
                }

                HttpContext.Session.SetInt32(SessionKeys.MemberId, memberId);

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("", "API'ye bağlanılamadı. API çalışıyor mu kontrol et.");
                return View(model);
            }
            catch (TaskCanceledException)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("", "İstek zaman aşımına uğradı. Tekrar deneyin.");
                return View(model);
            }
            catch (Exception)
            {
                HttpContext.Session.Clear();
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterUserDTO { ContractVersion = "v1" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var apiBaseUrl = _configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(apiBaseUrl))
            {
                ModelState.AddModelError("", "ApiSettings:BaseUrl ayarı bulunamadı.");
                return View(model);
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                using var response = await client.PostAsJsonAsync($"{apiBaseUrl}/api/auth/register", model);

                if (!response.IsSuccessStatusCode)
                {
                    var apiMsg = await TryReadMessageAsync(response);
                    ModelState.AddModelError("", apiMsg ?? "Kayıt başarısız.");
                    return View(model);
                }

                return RedirectToAction("Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "API'ye bağlanılamadı. API çalışıyor mu kontrol et.");
                return View(model);
            }
            catch (TaskCanceledException)
            {
                ModelState.AddModelError("", "İstek zaman aşımına uğradı. Tekrar deneyin.");
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Beklenmeyen bir hata oluştu.");
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
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
