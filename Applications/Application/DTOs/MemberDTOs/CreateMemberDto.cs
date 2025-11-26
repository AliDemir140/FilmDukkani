namespace Application.DTOs.MemberDTOs
{
    public class CreateMemberDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public string? Phone { get; set; }
    }
}
