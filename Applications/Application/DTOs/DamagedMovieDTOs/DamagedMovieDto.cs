using System;

namespace Application.DTOs.DamagedMovieDTOs
{
    public class DamagedMovieDto
    {
        public int Id { get; set; }

        public int MovieCopyId { get; set; }
        public string MovieTitle { get; set; }
        public string Barcode { get; set; }

        public string? Note { get; set; }

        public bool IsSentToPurchase { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
