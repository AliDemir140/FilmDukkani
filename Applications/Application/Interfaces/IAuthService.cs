using Application.DTOs.UserDTOs;

namespace Application.Abstractions
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterUserDTO dto);
        Task<(bool Success, string Message, LoginResponseDTO Data)> LoginAsync(LoginRequestDTO dto);
    }
}
