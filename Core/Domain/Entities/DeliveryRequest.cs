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

        // Teslimatın planlandığı gün
        public DateTime DeliveryDate { get; set; }

        // Enum status
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        public ICollection<DeliveryRequestItem> Items { get; set; } = new List<DeliveryRequestItem>();

        // İptal talebi alanları
        public string? CancelReason { get; set; }
        public DateTime? CancelRequestedAt { get; set; }
        public DeliveryStatus? CancelPreviousStatus { get; set; } // iptal reddedilirse geri dönmek için
        public DateTime? CancelDecisionAt { get; set; }
        public bool? CancelApproved { get; set; } // true=onay, false=red, null=bekliyor
    }
}
