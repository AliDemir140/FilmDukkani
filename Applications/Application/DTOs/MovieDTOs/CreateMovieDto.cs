using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovieDTOs
{
    public class CreateMovieDto
    {
        [Required(ErrorMessage = "Film adı zorunludur.")]
        [MaxLength(200, ErrorMessage = "Film adı en fazla 200 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Orijinal ad en fazla 200 karakter olabilir.")]
        public string? OriginalTitle { get; set; }

        [MaxLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        public string? Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Yapım yılı 1900 ile 2100 arasında olmalıdır.")]
        public int ReleaseYear { get; set; }

        [MaxLength(500, ErrorMessage = "Teknik özellikler en fazla 500 karakter olabilir.")]
        public string? TechnicalDetails { get; set; }

        [MaxLength(300, ErrorMessage = "Ses özellikleri en fazla 300 karakter olabilir.")]
        public string? AudioFeatures { get; set; }

        [MaxLength(300, ErrorMessage = "Altyazı bilgisi en fazla 300 karakter olabilir.")]
        public string? SubtitleLanguages { get; set; }

        [MaxLength(500, ErrorMessage = "Fragman linki en fazla 500 karakter olabilir.")]
        public string? TrailerUrl { get; set; }

        [MaxLength(500, ErrorMessage = "Kapak resmi linki en fazla 500 karakter olabilir.")]
        public string? CoverImageUrl { get; set; }

        [MaxLength(50, ErrorMessage = "Barkod en fazla 50 karakter olabilir.")]
        public string? Barcode { get; set; }

        [MaxLength(100, ErrorMessage = "Tedarikçi adı en fazla 100 karakter olabilir.")]
        public string? Supplier { get; set; }

        public bool IsEditorsChoice { get; set; }
        public bool IsNewRelease { get; set; }
        public bool IsAwardWinner { get; set; }

        [Required(ErrorMessage = "En az 1 kategori seçmelisiniz.")]
        public List<int> CategoryIds { get; set; } = new();

        public List<int> ActorIds { get; set; } = new();
        public List<int> DirectorIds { get; set; } = new();

        [MaxLength(1000, ErrorMessage = "Yeni oyuncular alanı en fazla 1000 karakter olabilir.")]
        public string? NewActors { get; set; }

        [MaxLength(1000, ErrorMessage = "Yeni yönetmenler alanı en fazla 1000 karakter olabilir.")]
        public string? NewDirectors { get; set; }
    }
}
