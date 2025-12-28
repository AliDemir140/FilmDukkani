using Domain.Enums;

namespace Application.DTOs.PurchaseRequestDTOs
{
    public class PurchaseRequestDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string? Note { get; set; }

        public PurchaseRequestStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? DecisionAt { get; set; }
        public string? DecisionNote { get; set; }
    }
}
