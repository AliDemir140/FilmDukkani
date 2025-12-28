using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CourierDTOs
{
    public class UpdateCourierDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
