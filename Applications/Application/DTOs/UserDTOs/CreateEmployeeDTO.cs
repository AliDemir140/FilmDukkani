using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDTOs
{
    public class CreateEmployeeDTO
    {
        [Required, MinLength(2), MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MinLength(2), MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Phone, MaxLength(20)]
        public string? Phone { get; set; }

        [Range(1, int.MaxValue)]
        public int MembershipPlanId { get; set; }

        [Required]
        public string Role { get; set; } = "User";
    }
}
