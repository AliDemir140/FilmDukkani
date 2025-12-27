namespace Domain.Entities
{
    public class Review : BaseEntity
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public byte Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
