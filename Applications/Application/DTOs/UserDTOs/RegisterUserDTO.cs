using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDTOs
{
    public class RegisterUserDTO
    {
        [Required, MinLength(2), MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MinLength(2), MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Required, Phone, MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string AddressLine { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string District { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? PostalCode { get; set; }

        [Range(typeof(bool), "true", "true")]
        public bool ContractAccepted { get; set; }

        [MaxLength(20)]
        public string? ContractVersion { get; set; }
    }
}
