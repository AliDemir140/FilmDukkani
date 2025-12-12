using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AccountingReportDTOs
{
    public class AccountingReportFilterDto
    {
        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }
    }
}
