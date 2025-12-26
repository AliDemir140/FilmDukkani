namespace Domain.Enums
{
    public enum DeliveryStatus : byte
    {
        Pending = 0,          // İstek oluşturuldu, henüz hazırlık yapılmadı
        Prepared = 1,         // PrepareTomorrowDeliveries çalıştı, filmler seçildi
        Shipped = 2,          // Kurye yola çıktı
        Delivered = 3,        // Filmler müşteriye teslim edildi
        Completed = 4,        // Süreç tamamen bitti

        CancelRequested = 5,  // Kullanıcı iptal talebi oluşturdu (admin onayı bekleniyor)
        Cancelled = 6         // Admin onayladı / iptal edildi
    }
}
