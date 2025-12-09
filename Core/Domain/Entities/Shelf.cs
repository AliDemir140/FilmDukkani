namespace Domain.Entities
{
    public class Shelf : BaseEntity
    {
        // Rafın adı (Örn: "A Blok - 1. Raf")
        public string Name { get; set; }

        // Depo içi konum kodu (opsiyonel, örn: "A1-01")
        public string? LocationCode { get; set; }

        // Bu raftaki fiziksel kopyalar
        public ICollection<MovieCopy> MovieCopies { get; set; } = new List<MovieCopy>();
    }
}
