namespace Application.DTOs.AccountingDTOs
{
    public class MovieProfitReportDto
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }

        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

        public int DeliveredCount { get; set; }
        public int DamagedCount { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }

    }
}
