using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.BillingDTOs
{
    public class ChargeMembershipDto
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int MembershipPlanId { get; set; }
    }
}
