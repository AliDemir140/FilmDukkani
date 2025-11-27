namespace Application.DTOs.MemberMovieListDTOs
{
    public class CreateMemberMovieListItemDto
    {
        public int MemberMovieListId { get; set; }
        public int MovieId { get; set; }

        public int Priority { get; set; }
    }
}
