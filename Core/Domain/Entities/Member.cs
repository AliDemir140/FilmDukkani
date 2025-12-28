using Domain.Enums;

namespace Domain.Entities
{
    public class Member : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string IdentityUserId { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public string AddressLine { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string? PostalCode { get; set; }

        public bool ContractAccepted { get; set; }
        public DateTime? ContractAcceptedAt { get; set; }
        public string? ContractVersion { get; set; }

        public int MembershipPlanId { get; set; }
        public MembershipPlan MembershipPlan { get; set; } = default!;

        public DateTime MembershipStartDate { get; set; }
        public MemberStatus Status { get; set; } = MemberStatus.Active;

        public ICollection<DeliveryRequest> DeliveryRequests { get; set; } = new List<DeliveryRequest>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

        public ICollection<BillingAttempt> BillingAttempts { get; set; } = new List<BillingAttempt>();
    }
}
