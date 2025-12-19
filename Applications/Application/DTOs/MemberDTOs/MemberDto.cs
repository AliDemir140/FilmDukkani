using System;

namespace Application.DTOs.MemberDTOs
{
    public class MemberDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public string? Phone { get; set; }

        public int MembershipPlanId { get; set; }
        public string MembershipPlanName { get; set; }

        public DateTime? MembershipStartDate { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
