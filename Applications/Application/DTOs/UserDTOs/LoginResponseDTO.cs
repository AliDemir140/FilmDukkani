namespace Application.DTOs.UserDTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }  
        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
