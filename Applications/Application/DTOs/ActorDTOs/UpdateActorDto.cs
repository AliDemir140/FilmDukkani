using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ActorDTOs
{
    public class UpdateActorDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(1000)]
        public string? Biography { get; set; }
    }
}
