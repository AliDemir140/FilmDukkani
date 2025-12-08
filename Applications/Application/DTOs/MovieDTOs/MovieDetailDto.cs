using System.Collections.Generic;

namespace Application.DTOs.MovieDTOs
{
    public class MovieDetailDto
    {
        public int Id { get; set; }

        // Temel bilgiler
        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }

        // Kategori bilgisi
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        // WORLD belgesine göre ileride doldurulacak alanlar
        public string OriginalTitle { get; set; }           // Orijinal adı
        public string TechnicalDetails { get; set; }        // Teknik özellikler
        public string AudioFeatures { get; set; }           // Ses özellikleri
        public string SubtitleLanguages { get; set; }       // Altyazı dilleri
        public string TrailerUrl { get; set; }              // Fragman linki
        public string CoverImageUrl { get; set; }           // Kapak resmi

        // Metadata tarafı: Oyuncu, yönetmen, ödül bilgileri
        public List<string> Actors { get; set; } = new();
        public List<string> Directors { get; set; } = new();

        public List<MovieAwardInfoDto> Awards { get; set; } = new();
    }

    public class MovieAwardInfoDto
    {
        public string AwardName { get; set; }       // Ödül adı (Oscar, Altın Küre vs.)
        public int? Year { get; set; }              // Yılı
        public string Category { get; set; }        // Ödül kategorisi
        public bool IsWinner { get; set; }          // Kazandı mı, aday mı
    }
}
