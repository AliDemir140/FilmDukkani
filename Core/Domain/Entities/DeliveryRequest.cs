using System;
using System.Collections.Generic;
using Domain.Enums;


namespace Domain.Entities
{
    public class DeliveryRequest : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        // Hangi listenin üzerinden gönderim yapılmış
        public int MemberMovieListId { get; set; }
        public MemberMovieList MemberMovieList { get; set; }

        // İsteğin sisteme düştüğü tarih
        public DateTime RequestedDate { get; set; }

        // Teslimatın planlandığı gün (WORLD: 12.10.2006 vs.)
        public DateTime DeliveryDate { get; set; }

        // Enum status
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        public ICollection<DeliveryRequestItem> Items { get; set; } = new List<DeliveryRequestItem>();
    }
}
