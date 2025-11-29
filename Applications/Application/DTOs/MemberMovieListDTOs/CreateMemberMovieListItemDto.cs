using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberMovieListDTOs
{
    public class CreateMemberMovieListItemDto
    {
        [Required(ErrorMessage = "Liste ID zorunludur.")]
        public int MemberMovieListId { get; set; }

        [Required(ErrorMessage = "Film ID zorunludur.")]
        public int MovieId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Öncelik en az 1 olmalıdır.")]
        public int Priority { get; set; }
    }
}
