using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MVC.Extensions;

namespace MVC.Services
{
    public class MemberContext : IMemberContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string TokenKey = "JWT_TOKEN";

        public MemberContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        public string? Token
        {
            get
            {
                return _httpContextAccessor.HttpContext?
                    .Session.GetString(TokenKey);
            }
        }

        public string? UserId =>
            GetClaim(ClaimTypes.NameIdentifier);

        public string? UserName =>
            GetClaim(ClaimTypes.Name);

        public string? Email =>
            GetClaim(ClaimTypes.Email);

        private string? GetClaim(string claimType)
        {
            if (string.IsNullOrEmpty(Token))
                return null;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);

            return jwt.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
        }
    }
}
