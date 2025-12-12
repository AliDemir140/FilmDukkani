namespace Application.DTOs.AccountingDTOs
{
    public class CategoryProfitReportDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int DeliveredCount { get; set; }
        public int DamagedCount { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }

    }
}
