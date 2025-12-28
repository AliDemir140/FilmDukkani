using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberDTOs
{
    public class UpdateMemberPlanDto
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int MembershipPlanId { get; set; }
    }
}
