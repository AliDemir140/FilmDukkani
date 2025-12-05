using Domain.Enums;


namespace Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; }      // Ad
        public string LastName { get; set; }       // Soyad
        public string Email { get; set; }          // Giriş / iletişim maili
        public string Password { get; set; }       // Şimdilik düz string, ileride hash 
        public string? Phone { get; set; }         // Telefon Numarası

        // Üyelik planı ilişkisi
        public int MembershipPlanId { get; set; }          // Üyenin planı

        public MembershipPlan MembershipPlan { get; set; } // Navigation property

        public DateTime MembershipStartDate { get; set; }  // Planın başladığı tarih
        public MemberStatus Status { get; set; } = MemberStatus.Active;

        public ICollection<DeliveryRequest> DeliveryRequests { get; set; } = new List<DeliveryRequest>();


    }
}
