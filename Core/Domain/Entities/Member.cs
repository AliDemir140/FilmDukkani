using Domain.Enums;

namespace Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        //Identity bağlantısı
        public string IdentityUserId { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public int MembershipPlanId { get; set; }
        public MembershipPlan MembershipPlan { get; set; } = default!;

        public DateTime MembershipStartDate { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.Active;

        public ICollection<DeliveryRequest> DeliveryRequests { get; set; } = new List<DeliveryRequest>();
    }
}
