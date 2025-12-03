using System;

namespace Domain.Entities
{
    public class DeliveryRequestItem : BaseEntity
    {
        public int DeliveryRequestId { get; set; }
        public DeliveryRequest DeliveryRequest { get; set; }

        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        // Hangi list item üzerinden seçildiğini bilmek için
        public int MemberMovieListItemId { get; set; }
        public MemberMovieListItem MemberMovieListItem { get; set; }

        public bool IsReturned { get; set; }      // İade edildi mi
        public DateTime? ReturnDate { get; set; } // İade tarihi
        public bool IsDamaged { get; set; }       // Bozuk/hatalı mı işaretlendi
    }
}
