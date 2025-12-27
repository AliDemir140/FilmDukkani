using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ReviewDTOs
{
    public class CreateReviewDto
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        public int MemberId { get; set; }

        [Range(1, 5)]
        public byte Rating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
