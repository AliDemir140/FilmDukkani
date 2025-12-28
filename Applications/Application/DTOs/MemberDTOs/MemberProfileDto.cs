using System;
using Domain.Enums;

namespace Application.DTOs.MemberDTOs
{
    public class MemberProfileDto
    {
        public int MemberId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int MembershipPlanId { get; set; }
        public string MembershipPlanName { get; set; } = string.Empty;

        public DateTime MembershipStartDate { get; set; }
        public MemberStatus Status { get; set; }
    }
}
