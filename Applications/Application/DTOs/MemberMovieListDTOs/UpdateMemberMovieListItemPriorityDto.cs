using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberMovieListDTOs
{
    public class UpdateMemberMovieListItemPriorityDto
    {
        [Required(ErrorMessage = "Liste öğesi ID zorunludur.")]
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Öncelik en az 1 olmalıdır.")]
        public int Priority { get; set; }
    }
}
