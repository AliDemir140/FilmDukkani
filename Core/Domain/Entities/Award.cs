namespace Domain.Entities
{
    public class Award : BaseEntity
    {
        public string Name { get; set; }              // Ödül adı (Oscars, Cannes vs.)
        public string? Organization { get; set; }     // Veren kurum (Academy, Festival vb.)
        public string? Description { get; set; }

        // İlişki: Bir ödül birçok filmle ilişkili olabilir
        public ICollection<MovieAward> MovieAwards { get; set; } = new List<MovieAward>();
    }
}
