namespace Application.DTOs.MemberDTOs
{
    public class UpdateMemberDto
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string? Phone { get; set; }

        public int MembershipPlanId { get; set; }  
        public DateTime? MembershipStartDate { get; set; } //Plan değiştirmek isterse

    }
}
