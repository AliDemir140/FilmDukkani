using Application.DTOs.AccountingDTOs;

namespace MVC.Areas.DashBoard.Models
{
    public class AccountingDashboardViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ProfitLossSummaryDto? ProfitLoss { get; set; }
        public List<MemberProfitReportDto> MemberReport { get; set; } = new();
        public List<MovieProfitReportDto> MovieReport { get; set; } = new();
        public List<CategoryProfitReportDto> CategoryReport { get; set; } = new();
    }
}
