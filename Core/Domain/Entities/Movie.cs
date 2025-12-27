using Domain.Enums;

namespace Domain.Entities
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; }
        public string? OriginalTitle { get; set; }
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }

        public string? TechnicalDetails { get; set; }
        public string? AudioFeatures { get; set; }
        public string? SubtitleLanguages { get; set; }
        public string? TrailerUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Barcode { get; set; }
        public string? Supplier { get; set; }

        public MovieStatus Status { get; set; } = MovieStatus.Available;

        public bool IsEditorsChoice { get; set; }
        public bool IsNewRelease { get; set; }
        public bool IsAwardWinner { get; set; }
        public ICollection<MovieCategory> MovieCategories { get; set; } = new List<MovieCategory>();

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
        public ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
        public ICollection<MovieAward> MovieAwards { get; set; } = new List<MovieAward>();
        public ICollection<DeliveryRequestItem> DeliveryRequestItems { get; set; } = new List<DeliveryRequestItem>();
        public ICollection<MovieCopy> MovieCopies { get; set; } = new List<MovieCopy>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
