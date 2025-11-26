namespace Application.DTOs.MovieDTOs
{
    public class UpdateMovieDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }

        public int CategoryId { get; set; }
    }
}
