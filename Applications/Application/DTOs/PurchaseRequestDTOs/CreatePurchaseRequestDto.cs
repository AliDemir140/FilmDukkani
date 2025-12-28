using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PurchaseRequestDTOs
{
    public class CreatePurchaseRequestDto
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; } = 1;

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
