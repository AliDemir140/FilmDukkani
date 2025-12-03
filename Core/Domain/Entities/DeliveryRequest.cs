using System;
using System.Collections.Generic;

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

        // Şimdilik byte tutuyoruz, #19’da enum’a bağlayacağız
        // 0: Pending, 1: Prepared, 2: OnTheWay, 3: Delivered, 4: Completed, 5: Cancelled
        public byte Status { get; set; }

        public ICollection<DeliveryRequestItem> Items { get; set; } = new List<DeliveryRequestItem>();
    }
}
