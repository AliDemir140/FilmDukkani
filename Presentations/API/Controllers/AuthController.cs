using Application.DTOs.UserDTOs;
using Application.ServiceManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserServiceManager _userServiceManager;

        public AuthController(UserServiceManager userServiceManager)
        {
            _userServiceManager = userServiceManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServiceManager.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServiceManager.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return Ok(new
            {
                userId,
                userName,
                role
            });
        }

        [HttpPost("set-role")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> SetRole([FromBody] SetUserRoleDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServiceManager.SetUserRoleAsync(dto.UserId, dto.Role);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("roles")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Roles()
        {
            var roles = await _userServiceManager.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("user-role")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UserRole([FromQuery] string userId)
        {
            var role = await _userServiceManager.GetUserRoleAsync(userId);
            if (string.IsNullOrWhiteSpace(role))
                return Ok("User");

            return Ok(role);
        }

        [HttpPost("create-employee")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userServiceManager.CreateEmployeeAsync(dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, userId = result.UserId });
        }

        [HttpGet("user-name")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UserName([FromQuery] string userId)
        {
            var userName = await _userServiceManager.GetUserNameAsync(userId);
            if (string.IsNullOrWhiteSpace(userName))
                return Ok(string.Empty);

            return Ok(userName);
        }

    }
}
