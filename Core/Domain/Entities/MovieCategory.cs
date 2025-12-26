namespace Domain.Entities
{
    public class MovieCategory
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
    }
}
