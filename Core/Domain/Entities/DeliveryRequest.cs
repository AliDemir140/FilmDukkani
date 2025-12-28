using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Entities
{
    public class DeliveryRequest : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int MemberMovieListId { get; set; }
        public MemberMovieList MemberMovieList { get; set; }

        public DateTime RequestedDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;

        public ICollection<DeliveryRequestItem> Items { get; set; } = new List<DeliveryRequestItem>();

        public int? CourierId { get; set; }
        public Courier? Courier { get; set; }

        public string? CancelReason { get; set; }
        public DateTime? CancelRequestedAt { get; set; }
        public DeliveryStatus? CancelPreviousStatus { get; set; }
        public DateTime? CancelDecisionAt { get; set; }
        public bool? CancelApproved { get; set; }
    }
}
