using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MovieDTOs
{
    public class MovieDto
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int ReleaseYear { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
