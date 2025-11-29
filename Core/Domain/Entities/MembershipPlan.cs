namespace Domain.Entities
{
    public class MembershipPlan : BaseEntity
    {
        public string PlanName { get; set; }        // Ekonomik, 3'lü, Gold vs.
        public decimal Price { get; set; }          // Aylık ücret
        public int MaxMoviesPerMonth { get; set; }  // Aylık yaklaşık film sayısı (16, 24, 32, 40)
        public int MaxChangePerMonth { get; set; }  // Bir değişimde kaç film (2,3,4,5)
        public string? Description { get; set; }    // İsteğe bağlı açıklama

        public ICollection<Member> Members { get; set; } = new List<Member>(); // Bu üyelik planına bağlı olan tüm üyelerin listesi (1 plan → birçok üye)

    }
}
