namespace Domain.Entities
{
    public class DamagedMovie : BaseEntity
    {
        public int MovieCopyId { get; set; }
        public MovieCopy MovieCopy { get; set; }

        // Depo görevlisinin notu (örn: "disk çizik, son 10dk okunmuyor")
        public string? Note { get; set; }

        // Satın alma sorumlusuna iletildi mi / işlem yapıldı mı?
        public bool IsSentToPurchase { get; set; }
    }
}
