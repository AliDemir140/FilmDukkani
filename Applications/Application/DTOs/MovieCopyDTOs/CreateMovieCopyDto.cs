using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovieCopyDTOs
{
    public class CreateMovieCopyDto
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; }

        public int? ShelfId { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsDamaged { get; set; } = false;
    }
}
