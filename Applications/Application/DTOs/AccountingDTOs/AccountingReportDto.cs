namespace Application.DTOs.AccountingDTOs
{
    public class AccountingReportDto
    {
        public int ActiveMemberCount { get; set; }
        public int DeliveredMovieCount { get; set; }
        public int DamagedMovieCount { get; set; }

        public decimal EstimatedIncome { get; set; }
        public decimal EstimatedDamageCost { get; set; }
    }
}
