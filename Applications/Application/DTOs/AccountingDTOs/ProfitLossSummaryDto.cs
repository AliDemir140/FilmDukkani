using System;

namespace Application.DTOs.AccountingDTOs
{
    public class ProfitLossSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public decimal TotalRevenue { get; set; } // Toplam gelir
        public decimal TotalCost { get; set; } // Toplam maliyet
        public decimal Profit { get; set; } // Kar (Revenue - Cost)
    }
}
