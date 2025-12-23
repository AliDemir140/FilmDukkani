using Application.DTOs.UserDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserServiceManager _userServiceManager;

        public AuthController(UserServiceManager userServiceManager)
        {
            _userServiceManager = userServiceManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServiceManager.LoginAsync(model);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("validate-token")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.Identity?.Name;

            return Ok(new
            {
                message = "Token geçerli",
                userId,
                email
            });
        }
    }
}
