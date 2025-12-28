using Domain.Enums;

namespace Domain.Entities
{
    public class PurchaseRequest : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public int MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public int Quantity { get; set; }
        public string? Note { get; set; }

        public PurchaseRequestStatus Status { get; set; } = PurchaseRequestStatus.Pending;

        public DateTime? DecisionAt { get; set; }
        public string? DecisionNote { get; set; }
    }
}
