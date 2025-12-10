using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ShelfDTOs
{
    public class UpdateShelfDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string? LocationCode { get; set; }
    }
}
