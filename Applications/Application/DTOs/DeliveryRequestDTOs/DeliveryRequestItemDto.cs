using System;

namespace Application.DTOs.DeliveryRequestDTOs
{
    public class DeliveryRequestItemDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; }

        public bool IsReturned { get; set; }
        public DateTime? ReturnDate { get; set; }

        public bool IsDamaged { get; set; }
    }
}
