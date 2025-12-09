using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AwardDTOs
{
    public class UpdateAwardDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string AwardName { get; set; }

        [MaxLength(150)]
        public string? Organization { get; set; }
    }
}
