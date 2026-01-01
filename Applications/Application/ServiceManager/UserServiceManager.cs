using Application.Interfaces;
using Application.DTOs.UserDTOs;
using Application.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.ServiceManager
{
    public class UserServiceManager : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IMemberRepository _memberRepository;

        public UserServiceManager(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMemberRepository memberRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _memberRepository = memberRepository;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO dto)
        {
            if (!dto.ContractAccepted)
                return (false, "Sözleşme onayı zorunludur.");

            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return (false, "Bu email adresi zaten kullanılıyor.");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return (false, "Bu kullanıcı adı zaten kullanılıyor.");

            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            var member = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                IdentityUserId = user.Id,

                MembershipPlanId = 1,
                MembershipStartDate = DateTime.Today,
                Status = MemberStatus.PaymentDue,

                AddressLine = dto.AddressLine,
                City = dto.City,
                District = dto.District,
                PostalCode = dto.PostalCode,

                ContractAccepted = true,
                ContractAcceptedAt = DateTime.UtcNow,
                ContractVersion = string.IsNullOrWhiteSpace(dto.ContractVersion) ? "v1" : dto.ContractVersion.Trim()
            };

            await _memberRepository.AddAsync(member);

            return (true, "Kullanıcı başarıyla oluşturuldu.");
        }

        public async Task<(bool Success, string Message, LoginResponseDTO Data)> LoginAsync(LoginRequestDTO dto)
        {
            IdentityUser? user =
                await _userManager.FindByEmailAsync(dto.UserNameOrEmail)
                ?? await _userManager.FindByNameAsync(dto.UserNameOrEmail);

            if (user == null)
                return (false, "Kullanıcı bulunamadı.", null);

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: false
            );

            if (!signInResult.Succeeded)
                return (false, "Şifre hatalı.", null);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            var token = GenerateToken(user, role);

            var response = new LoginResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                Role = role
            };

            return (true, "Giriş başarılı.", response);
        }

        private string GenerateToken(IdentityUser user, string role)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["Key"])
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.Role, role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<(bool Success, string Message)> SetUserRoleAsync(string userId, string role)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return (false, "UserId zorunludur.");

            if (string.IsNullOrWhiteSpace(role))
                return (false, "Role zorunludur.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Kullanıcı bulunamadı.");

            if (!await _roleManager.RoleExistsAsync(role))
                return (false, "Rol bulunamadı.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles != null && currentRoles.Count > 0)
            {
                var remove = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!remove.Succeeded)
                    return (false, string.Join(", ", remove.Errors.Select(e => e.Description)));
            }

            var add = await _userManager.AddToRoleAsync(user, role);
            if (!add.Succeeded)
                return (false, string.Join(", ", add.Errors.Select(e => e.Description)));

            return (true, "Rol güncellendi.");
        }

        public async Task<List<string>> GetAllRolesAsync()
        {
            var roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return await Task.FromResult(roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x!).ToList());
        }

        public async Task<string> GetUserRoleAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return string.Empty;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return string.Empty;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.FirstOrDefault() ?? "User";
        }

        public async Task<(bool Success, string Message, string UserId)> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return (false, "Bu email adresi zaten kullanılıyor.", string.Empty);

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return (false, "Bu kullanıcı adı zaten kullanılıyor.", string.Empty);

            if (!await _roleManager.RoleExistsAsync(dto.Role))
                return (false, "Rol bulunamadı.", string.Empty);

            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.Phone
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)), string.Empty);

            var addRole = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!addRole.Succeeded)
                return (false, string.Join(", ", addRole.Errors.Select(e => e.Description)), string.Empty);

            var member = new Member
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                IdentityUserId = user.Id,

                MembershipPlanId = dto.MembershipPlanId,
                MembershipStartDate = DateTime.Today,
                Status = MemberStatus.Active,

                AddressLine = "Dashboard",
                City = "Dashboard",
                District = "Dashboard",

                ContractAccepted = true,
                ContractAcceptedAt = DateTime.UtcNow,
                ContractVersion = "dashboard"
            };

            await _memberRepository.AddAsync(member);

            return (true, "Çalışan kullanıcı oluşturuldu.", user.Id);
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return string.Empty;

            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName ?? string.Empty;
        }

    }
}
