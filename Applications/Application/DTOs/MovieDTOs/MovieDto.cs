namespace Application.DTOs.MovieDTOs
{
    public class MovieDto
    {
        public int Id { get; set; }

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

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
