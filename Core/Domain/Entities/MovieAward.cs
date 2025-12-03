namespace Domain.Entities
{
    public class MovieAward : BaseEntity
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int AwardId { get; set; }
        public Award Award { get; set; }

        public int? Year { get; set; }          // Ödül yılı
        public string? Category { get; set; }   // En iyi film, en iyi oyuncu vs.
        public bool IsWinner { get; set; }      // Aday mı, kazandı mı
    }
}
