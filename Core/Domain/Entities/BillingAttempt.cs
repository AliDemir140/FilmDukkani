namespace Domain.Entities
{
    public class BillingAttempt : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;

        public string Period { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public bool Success { get; set; }

        public string? Error { get; set; }

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
    }
}
