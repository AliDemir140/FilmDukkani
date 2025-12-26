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

        // REGISTER
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO dto)
        {
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                return (false, "Bu email adresi zaten kullanılıyor.");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return (false, "Bu kullanıcı adı zaten kullanılıyor.");

            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));

            // DEFAULT ROLE
            if (!await _roleManager.RoleExistsAsync("User"))
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            // MEMBER OLUŞTUR
            var member = new Member
            {
                FirstName = dto.UserName,
                LastName = dto.UserName,
                Email = dto.Email,
                IdentityUserId = user.Id,
                MembershipPlanId = 1, // default plan
                MembershipStartDate = DateTime.Today,
                Status = MemberStatus.Active
            };

            await _memberRepository.AddAsync(member);

            return (true, "Kullanıcı başarıyla oluşturuldu.");
        }

        // LOGIN
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
                UserName = user.UserName,
                Role = role
            };

            return (true, "Giriş başarılı.", response);
        }

        // JWT
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
    }
}
