using System;

namespace Application.DTOs.AwardDTOs
{
    public class AwardDto
    {
        public int Id { get; set; }

        public string AwardName { get; set; }
        public string? Organization { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
