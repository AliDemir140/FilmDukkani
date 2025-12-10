using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovieCopyDTOs
{
    public class UpdateMovieCopyDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Barcode { get; set; }

        public int? ShelfId { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsDamaged { get; set; }
    }
}
