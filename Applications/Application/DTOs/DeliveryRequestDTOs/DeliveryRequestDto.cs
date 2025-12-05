using System;
using System.Collections.Generic;
using Domain.Enums;


namespace Application.DTOs.DeliveryRequestDTOs
{
    public class DeliveryRequestDto
    {
        public int Id { get; set; }

        public int MemberId { get; set; }
        public string MemberFullName { get; set; }

        public int MemberMovieListId { get; set; }
        public string ListName { get; set; }

        public DateTime RequestedDate { get; set; }
        public DateTime DeliveryDate { get; set; }

        public DeliveryStatus Status { get; set; }

        public List<DeliveryRequestItemDto> Items { get; set; } = new();
    }
}
