namespace Application.DTOs.AccountingReportDTOs
{
    public class AccountingSummaryDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public int TotalDeliveryRequests { get; set; }
        public int PreparedDeliveryRequests { get; set; }
        public int CompletedDeliveryRequests { get; set; }
        public int CancelledDeliveryRequests { get; set; }

        public int TotalDeliveredItems { get; set; }
        public int ReturnedItems { get; set; }
        public int DamagedItems { get; set; }

        public decimal EstimatedMembershipRevenue { get; set; }
    }
}
