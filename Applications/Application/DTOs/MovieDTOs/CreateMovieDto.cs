using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovieDTOs
{
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Film adı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Film adı en fazla 200 karakter olabilir.")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Yapım yılı 1900 ile 2100 arasında olmalıdır.")]
        public int ReleaseYear { get; set; }

        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int CategoryId { get; set; }
    }
}
