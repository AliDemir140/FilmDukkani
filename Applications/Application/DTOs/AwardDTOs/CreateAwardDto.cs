using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AwardDTOs
{
    public class CreateAwardDto
    {
        [Required]
        [MaxLength(150)]
        public string AwardName { get; set; }

        [MaxLength(150)]
        public string? Organization { get; set; }
    }
}
