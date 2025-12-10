using System;

namespace Application.DTOs.MovieCopyDTOs
{
    public class MovieCopyDto
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        public string MovieTitle { get; set; }

        public string Barcode { get; set; }

        public int? ShelfId { get; set; }
        public string? ShelfName { get; set; }

        public bool IsAvailable { get; set; }
        public bool IsDamaged { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
