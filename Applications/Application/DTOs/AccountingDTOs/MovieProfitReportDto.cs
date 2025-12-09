namespace Application.DTOs.AccountingDTOs
{
    public class MovieProfitReportDto
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Profit { get; set; }

        public int RentalCount { get; set; } // Kaç kere gönderilmiş
    }
}
