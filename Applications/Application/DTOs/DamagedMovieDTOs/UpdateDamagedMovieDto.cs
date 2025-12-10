using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DamagedMovieDTOs
{
    public class UpdateDamagedMovieDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int MovieCopyId { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public bool IsSentToPurchase { get; set; }
    }
}
