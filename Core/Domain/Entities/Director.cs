namespace Domain.Entities
{
    public class Director : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string? Biography { get; set; }

        // İlişki: Bir yönetmen birçok film yönetebilir (M:N)
        public ICollection<MovieDirector> MovieDirectors { get; set; } = new List<MovieDirector>();
    }
}
