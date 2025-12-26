namespace Application.DTOs.MovieDTOs
{
    public class MovieDetailDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ReleaseYear { get; set; }

        // Geriye dönük uyumluluk
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        // Many-to-Many
        public List<int> CategoryIds { get; set; } = new();
        public List<string> CategoryNames { get; set; } = new();

        public string? OriginalTitle { get; set; }
        public string? TechnicalDetails { get; set; }
        public string? AudioFeatures { get; set; }
        public string? SubtitleLanguages { get; set; }
        public string? TrailerUrl { get; set; }
        public string? CoverImageUrl { get; set; }

        public List<string> Actors { get; set; } = new();
        public List<string> Directors { get; set; } = new();
        public List<MovieAwardInfoDto> Awards { get; set; } = new();
    }

    public class MovieAwardInfoDto
    {
        public string AwardName { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsWinner { get; set; }
    }
}
