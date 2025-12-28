using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.PurchaseRequestDTOs
{
    public class DecidePurchaseRequestDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required]
        public bool Approved { get; set; }

        [MaxLength(500)]
        public string? AdminNote { get; set; }
    }
}
