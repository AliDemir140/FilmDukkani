using System;

namespace Application.DTOs.AccountingDTOs
{
    public class MemberProfitReportDto
    {
        public int MemberId { get; set; }
        public string MemberFullName { get; set; }

        public int MembershipPlanId { get; set; }
        public string? MembershipPlanName { get; set; }

        public int DeliveredMovieCount { get; set; }
        public int DamagedMovieCount { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }

    }
}
