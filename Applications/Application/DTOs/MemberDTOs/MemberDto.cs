using System;

namespace Application.DTOs.MemberDTOs
{
    public class MemberDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public int MembershipPlanId { get; set; }
        public string MembershipPlanName { get; set; } = string.Empty;

        public DateTime? MembershipStartDate { get; set; }

        public string IdentityUserId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";
    }
}
