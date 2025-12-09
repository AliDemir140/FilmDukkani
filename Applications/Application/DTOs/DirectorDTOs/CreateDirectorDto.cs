using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DirectorDTOs
{
    public class CreateDirectorDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(1000)]
        public string? Biography { get; set; }
    }
}
