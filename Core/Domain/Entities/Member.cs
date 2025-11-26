namespace Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; }      // Ad
        public string LastName { get; set; }       // Soyad
        public string Email { get; set; }          // Giriş / iletişim maili
        public string Password { get; set; }       // Şimdilik düz string, ileride hash 
        public string? Phone { get; set; }         // Telefon Numarası
    }
}
