namespace Domain.Entities
{
    public class MemberMovieList : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public string Name { get; set; }

        public ICollection<MemberMovieListItem> Items { get; set; }
    }
}
