namespace Domain.Entities
{
    public class MovieActor : BaseEntity
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int ActorId { get; set; }
        public Actor Actor { get; set; }

        // İleride rol tipi, karakter adı vs. eklenebilir
        public string? RoleName { get; set; }
    }
}
