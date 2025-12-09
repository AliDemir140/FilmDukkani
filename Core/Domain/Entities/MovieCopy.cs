namespace Domain.Entities
{
    public class MovieCopy : BaseEntity
    {
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Barkod (zarflara basılacak, depo operasyonunda kullanılacak)
        public string Barcode { get; set; }

        public int? ShelfId { get; set; }
        public Shelf? Shelf { get; set; }

        // Şu an rafta / sistemde kullanılabilir mi?
        public bool IsAvailable { get; set; }

        // Bozuk mu?
        public bool IsDamaged { get; set; }

        // Bu kopyaya ait bozukluk kayıtları
        public ICollection<DamagedMovie> DamagedMovies { get; set; } = new List<DamagedMovie>();
    }
}
