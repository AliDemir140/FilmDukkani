namespace Domain.Entities
{
    public class Actor : BaseEntity
    {
        public string FirstName { get; set; }   // Ad
        public string LastName { get; set; }   // Soyad

        // İleride ihtiyaç olursa doldururuz (Doğum tarihi, ülke vs.)
        public string? Biography { get; set; }

        public ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }
}
