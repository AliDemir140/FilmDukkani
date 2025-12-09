using System;

namespace Application.DTOs.AccountingDTOs
{
    public class MemberProfitReportDto
    {
        public int MemberId { get; set; }
        public string MemberFullName { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }

        public int DeliveryCount { get; set; } // Teslimat sayısı
        public int MovieCount { get; set; } // Gönderilen film sayısı
    }
}
