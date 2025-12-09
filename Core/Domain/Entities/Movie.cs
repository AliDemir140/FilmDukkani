using Domain.Entities;
using Domain.Enums;

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

        public MovieStatus Status { get; set; } = MovieStatus.Available;

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
        public ICollection<MovieAward> MovieAwards { get; set; } = new List<MovieAward>();
        public ICollection<DeliveryRequestItem> DeliveryRequestItems { get; set; } = new List<DeliveryRequestItem>();
        public ICollection<MovieCopy> MovieCopies { get; set; } = new List<MovieCopy>();



    }
}
