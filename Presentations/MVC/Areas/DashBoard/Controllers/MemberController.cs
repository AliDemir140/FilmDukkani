using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.Constants;
using Application.DTOs.MemberDTOs;
using Application.DTOs.UserDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Mvc;
using MVC.Constants;
using MVC.Filters;

namespace MVC.Areas.DashBoard.Controllers
{
    [Area("DashBoard")]
    [RequireRole(RoleNames.Admin)]
    public class MemberController : Controller
    {
        private readonly MemberServiceManager _memberService;
        private readonly MembershipPlanServiceManager _planService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MemberController(
            MemberServiceManager memberService,
            MembershipPlanServiceManager planService,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _memberService = memberService;
            _planService = planService;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        private string? ApiBaseUrl => _configuration["ApiSettings:BaseUrl"];

        public async Task<IActionResult> Index()
        {
            var members = await _memberService.GetMembersAsync();
            await FillIdentityInfoForIndexAsync(members);
            return View(members);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Plans = await _planService.GetPlansAsync();
            await FillRolesForCreateAsync();
            return View(new CreateMemberDto { Role = RoleNames.User });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberDto dto)
        {
            ViewBag.Plans = await _planService.GetPlansAsync();
            await FillRolesForCreateAsync();

            if (!ModelState.IsValid)
                return View(dto);

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                TempData["Error"] = "ApiSettings:BaseUrl bulunamadı.";
                return View(dto);
            }

            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return RedirectToAction("Login", "Auth", new { area = "" });

            var baseUrl = ApiBaseUrl.TrimEnd('/');

            var payload = new CreateEmployeeDTO
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = BuildUserName(dto.Email),
                Password = dto.Password,
                Phone = dto.Phone,
                MembershipPlanId = dto.MembershipPlanId,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? RoleNames.User : dto.Role
            };

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage? res = null;
                string raw = string.Empty;

                for (int attempt = 1; attempt <= 3; attempt++)
                {
                    res = await client.PostAsJsonAsync($"{baseUrl}/api/auth/create-employee", payload);
                    raw = await res.Content.ReadAsStringAsync();

                    if (res.IsSuccessStatusCode)
                        break;

                    if ((int)res.StatusCode == 400 &&
                        raw.Contains("kullanıcı adı", StringComparison.OrdinalIgnoreCase))
                    {
                        payload.UserName = BuildUserName(dto.Email, attempt);
                        continue;
                    }

                    break;
                }

                if (res == null || !res.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Kullanıcı oluşturulamadı. (HTTP {(int)(res?.StatusCode ?? 0)}) {raw}";
                    return View(dto);
                }

                TempData["Success"] = "Üye oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"API'ye bağlanılamadı. {ex.Message}";
                return View(dto);
            }
        }

        private Task FillRolesForCreateAsync()
        {
            ViewBag.Roles = new List<string>
            {
                RoleNames.User,
                RoleNames.Admin,
                RoleNames.Accounting,
                RoleNames.Warehouse,
                RoleNames.Purchasing
            };

            return Task.CompletedTask;
        }

        private async Task FillIdentityInfoForIndexAsync(List<MemberDto> members)
        {
            if (members == null || members.Count == 0)
                return;

            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return;

            var baseUrl = ApiBaseUrl.TrimEnd('/');

            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            foreach (var m in members)
            {
                if (string.IsNullOrWhiteSpace(m.IdentityUserId))
                {
                    m.Role = RoleNames.User;
                    m.UserName = string.Empty;
                    continue;
                }

                try
                {
                    var role = await client.GetFromJsonAsync<string>(
                        $"{baseUrl}/api/auth/user-role?userId={m.IdentityUserId}"
                    );

                    m.Role = string.IsNullOrWhiteSpace(role) ? RoleNames.User : role!;
                }
                catch
                {
                    m.Role = RoleNames.User;
                }

                try
                {
                    var userName = await client.GetFromJsonAsync<string>(
                        $"{baseUrl}/api/auth/user-name?userId={m.IdentityUserId}"
                    );

                    m.UserName = userName ?? string.Empty;
                }
                catch
                {
                    m.UserName = string.Empty;
                }
            }
        }

        private static string BuildUserName(string email, int attempt = 0)
        {
            if (string.IsNullOrWhiteSpace(email))
                return attempt <= 0 ? "user" : $"user{attempt}";

            var at = email.IndexOf('@');
            var raw = at > 0 ? email.Substring(0, at) : email;
            raw = raw.Trim();

            if (raw.Length < 3)
                raw = raw + "001";

            if (attempt > 0)
                raw = $"{raw}{attempt}{DateTime.UtcNow:HHmmss}";

            return raw;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _memberService.GetMember(id);
            if (dto == null)
                return NotFound();

            ViewBag.Plans = await _planService.GetPlansAsync();
            await FillRolesForEditAsync(dto);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateMemberDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Plans = await _planService.GetPlansAsync();
                await FillRolesForEditAsync(dto);
                return View(dto);
            }

            var ok = await _memberService.UpdateMember(dto);
            if (!ok)
                return NotFound();

            var roleOk = await TrySetUserRoleAsync(dto.IdentityUserId, dto.Role);

            TempData[ok ? "Success" : "Error"] = ok ? "Üye güncellendi." : "Üye güncellenemedi.";
            if (!roleOk)
                TempData["Error"] = "Üye güncellendi ama rol güncellenemedi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var dto = await _memberService.GetMember(id);
            if (dto == null)
                return NotFound();

            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Id)
        {
            var ok = await _memberService.DeleteMember(Id);
            if (!ok)
                return NotFound();

            TempData["Success"] = "Üye silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task FillRolesForEditAsync(UpdateMemberDto dto)
        {
            var roles = await TryGetAllRolesAsync();

            var baseRoles = new List<string>
            {
                RoleNames.User,
                RoleNames.Admin,
                RoleNames.Accounting,
                RoleNames.Warehouse,
                RoleNames.Purchasing
            };

            foreach (var r in roles)
            {
                if (!baseRoles.Contains(r))
                    baseRoles.Add(r);
            }

            ViewBag.Roles = baseRoles;

            if (string.IsNullOrWhiteSpace(dto.Role) && !string.IsNullOrWhiteSpace(dto.IdentityUserId))
            {
                var current = await TryGetUserRoleAsync(dto.IdentityUserId);
                dto.Role = string.IsNullOrWhiteSpace(current) ? RoleNames.User : current;
            }

            if (string.IsNullOrWhiteSpace(dto.Role))
                dto.Role = RoleNames.User;
        }

        private async Task<List<string>> TryGetAllRolesAsync()
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return new List<string>();

            var baseUrl = ApiBaseUrl.TrimEnd('/');

            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return new List<string>();

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var roles = await client.GetFromJsonAsync<List<string>>($"{baseUrl}/api/auth/roles");
                return roles ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }

        private async Task<string> TryGetUserRoleAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return string.Empty;

            var baseUrl = ApiBaseUrl.TrimEnd('/');

            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var role = await client.GetFromJsonAsync<string>($"{baseUrl}/api/auth/user-role?userId={userId}");
                return role ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<bool> TrySetUserRoleAsync(string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
                return false;

            var baseUrl = ApiBaseUrl.TrimEnd('/');

            if (string.IsNullOrWhiteSpace(userId))
                return false;

            if (string.IsNullOrWhiteSpace(role))
                role = RoleNames.User;

            var token = HttpContext.Session.GetString(SessionKeys.JwtToken);
            if (string.IsNullOrWhiteSpace(token))
                return false;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var res = await client.PostAsJsonAsync($"{baseUrl}/api/auth/set-role",
                    new SetUserRoleDTO { UserId = userId, Role = role });

                return res.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
