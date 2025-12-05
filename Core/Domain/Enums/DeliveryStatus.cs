namespace Domain.Enums
{
    public enum DeliveryStatus : byte
    {
        Pending = 0,        // İstek oluşturuldu, henüz hazırlık yapılmadı
        Prepared = 1,       // PrepareTomorrowDeliveries çalıştı, filmler seçildi
        Shipped = 2,        // Kurye yola çıktı
        Delivered = 3,      // Filmler müşteriye teslim edildi
        Completed = 4,      // Süreç tamamen bitti
        Cancelled = 5       // Kullanıcı iptal etti
    }
}
