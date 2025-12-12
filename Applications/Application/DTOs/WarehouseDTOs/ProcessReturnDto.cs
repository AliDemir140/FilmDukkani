using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.WarehouseDTOs
{
    public class ProcessReturnDto
    {
        [Required]
        public int MovieCopyId { get; set; }

        // Film bozuk mu geldi?
        public bool IsDamaged { get; set; }

        // Bozuksa açıklama
        [MaxLength(500)]
        public string? Note { get; set; }

        // Raf değişikliği yapılacaksa
        public int? TargetShelfId { get; set; }
    }
}
