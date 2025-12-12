using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.WarehouseDTOs
{
    public class MoveCopyToShelfDto
    {
        [Required]
        public int MovieCopyId { get; set; }

        [Required]
        public int ShelfId { get; set; }
    }
}
