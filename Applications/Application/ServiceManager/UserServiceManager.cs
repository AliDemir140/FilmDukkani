using Application.Abstractions;
using Application.DTOs.UserDTOs;
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
        private readonly IConfiguration _configuration;

        public UserServiceManager(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        // REGISTER

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO dto)
        {
            var existingUserByEmail = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUserByEmail != null)
                return (false, "Bu email adresi zaten kullanılıyor.");

            var existingUserByUsername = await _userManager.FindByNameAsync(dto.UserName);
            if (existingUserByUsername != null)
                return (false, "Bu kullanıcı adı zaten kullanılıyor.");

            var user = new IdentityUser
            {
                UserName = dto.UserName,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

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

            var token = GenerateToken(user);

            var response = new LoginResponseDTO
            {
                Token = token,
                UserId = user.Id,
                UserName = user.UserName
            };

            return (true, "Giriş başarılı.", response);
        }


        // JWT TOKEN

        private string GenerateToken(IdentityUser user)
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
                new Claim(ClaimTypes.Email, user.Email ?? "")
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
