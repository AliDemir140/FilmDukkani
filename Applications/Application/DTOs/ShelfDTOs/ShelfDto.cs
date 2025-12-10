using System;

namespace Application.DTOs.ShelfDTOs
{
    public class ShelfDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? LocationCode { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
