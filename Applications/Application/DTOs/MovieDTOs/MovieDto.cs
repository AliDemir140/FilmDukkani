namespace Application.DTOs.MovieDTOs
{
    public class MovieDto
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int ReleaseYear { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
