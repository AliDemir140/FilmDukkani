using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDTOs
{
    public class SetUserRoleDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
