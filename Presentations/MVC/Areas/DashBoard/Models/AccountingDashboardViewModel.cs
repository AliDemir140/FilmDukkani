namespace MVC.Areas.DashBoard.Models
{
    public class AccountingDashboardViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string ProfitLossJson { get; set; } = string.Empty;
        public string MemberReportJson { get; set; } = string.Empty;
        public string MovieReportJson { get; set; } = string.Empty;
        public string CategoryReportJson { get; set; } = string.Empty;
    }
}
