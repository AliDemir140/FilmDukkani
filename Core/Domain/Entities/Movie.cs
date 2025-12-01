using Domain.Entities;

namespace Domain.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; }                  // Filmin adı
        public string? OriginalTitle { get; set; }         // Yabancı ise orijinal adı
        public string? Description { get; set; }           // Özeti
        public int ReleaseYear { get; set; }               // Yapım yılı

        public string? TechnicalDetails { get; set; }      // Teknik özellikler
        public string? AudioFeatures { get; set; }         // Ses özellikleri
        public string? SubtitleLanguages { get; set; }     // Altyazı dilleri (TR, EN vb.)
        public string? TrailerUrl { get; set; }            // Fragman linki
        public string? CoverImageUrl { get; set; }         // Kapak resmi linki
        public string? Barcode { get; set; }               // Barkod numarası
        public string? Supplier { get; set; }              // Tedarikçi firma adı (Tiglon vb.)

        public int CategoryId { get; set; }                // Şimdilik tek kategori
        public Category Category { get; set; }

        // Issue #15’te (Oyuncu, Yönetmen, Ödül) koleksiyon navigation’lar eklenecek
    }
}
