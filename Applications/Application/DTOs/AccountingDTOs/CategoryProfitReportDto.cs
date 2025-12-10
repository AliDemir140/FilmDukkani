namespace Application.DTOs.AccountingDTOs
{
    public class CategoryProfitReportDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public decimal TotalRevenue { get; set; }   // Toplam Gelir
        public decimal TotalCost { get; set; }  // Toplam Maliyet
        public decimal Profit { get; set; }     // Kar, Kazanç

        public int RentalCount { get; set; }    // Kira Sayısı
    }
}
