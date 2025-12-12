using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DeliveryRequestDTOs
{
    public class ReturnDeliveryItemDto
    {
        [Required]
        public int DeliveryRequestItemId { get; set; }

        public bool IsDamaged { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
