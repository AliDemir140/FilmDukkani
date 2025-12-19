using System;

namespace Application.DTOs.MemberMovieListDTOs
{
    public class MemberMovieListItemDto
    {
        public int Id { get; set; }
        public int MemberMovieListId { get; set; }

        public int MovieId { get; set; }
        public string? MovieTitle { get; set; }

        public int Priority { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
