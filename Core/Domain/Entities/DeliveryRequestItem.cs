namespace Domain.Entities
{
    public class DeliveryRequestItem : BaseEntity
    {
        public int DeliveryRequestId { get; set; }
        public DeliveryRequest DeliveryRequest { get; set; }

        // Kopya bazlı takip
        public int MovieCopyId { get; set; }
        public MovieCopy MovieCopy { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int MemberMovieListItemId { get; set; }
        public MemberMovieListItem MemberMovieListItem { get; set; }

        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsDamaged { get; set; }
    }
}
