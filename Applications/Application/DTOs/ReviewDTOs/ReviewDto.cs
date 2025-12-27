namespace Application.DTOs.ReviewDTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int MemberId { get; set; }

        public string MemberFullName { get; set; } = string.Empty;

        public byte Rating { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsOwner { get; set; }
    }
}
