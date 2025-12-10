using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DamagedMovieDTOs
{
    public class CreateDamagedMovieDto
    {
        [Required]
        public int MovieCopyId { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
