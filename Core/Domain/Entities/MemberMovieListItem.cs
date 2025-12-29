// DOSYA: Domain/Entities/MemberMovieListItem.cs
namespace Domain.Entities
{
    public class MemberMovieListItem : BaseEntity
    {
        public int MemberMovieListId { get; set; }
        public MemberMovieList MemberMovieList { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int Priority { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Shipped olduğunda listede görünmesin diye rezerve edilir.
        public bool IsReserved { get; set; } = false;
    }
}
