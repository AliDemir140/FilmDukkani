namespace Domain.Entities
{
    public class MemberMovieListItem : BaseEntity
    {
        public int MemberMovieListId { get; set; }
        public MemberMovieList MemberMovieList { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int Priority { get; set; }   // kaçıncı sırada?

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}
