namespace Domain.Enums
{
    public enum MovieStatus : byte
    {
        Available = 0,      // Depoda mevcut
        Rented = 1,         // Müşteriye gönderilmiş
        Damaged = 2,        // Bozuk, test bekliyor
        Lost = 3,           // iade gelmedi
        ComingSoon = 4      // Yeni gelecek film
    }
}
