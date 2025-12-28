using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Application.DTOs.DeliveryRequestDTOs
{
    public class DeliveryRequestDto
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public string MemberFullName { get; set; } = string.Empty;

        public string? MemberPhone { get; set; }
        public string? MemberAddressLine { get; set; }
        public string? MemberCity { get; set; }
        public string? MemberDistrict { get; set; }
        public string? MemberPostalCode { get; set; }

        public int MemberMovieListId { get; set; }
        public string ListName { get; set; } = string.Empty;

        public DateTime RequestedDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; }
        public string StatusText => Status.ToString();

        public int? CourierId { get; set; }
        public string? CourierFullName { get; set; }

        public string? CancelReason { get; set; }
        public DateTime? CancelRequestedAt { get; set; }
        public DeliveryStatus? CancelPreviousStatus { get; set; }
        public DateTime? CancelDecisionAt { get; set; }
        public bool? CancelApproved { get; set; }

        public List<DeliveryRequestItemDto> Items { get; set; } = new();
    }
}
