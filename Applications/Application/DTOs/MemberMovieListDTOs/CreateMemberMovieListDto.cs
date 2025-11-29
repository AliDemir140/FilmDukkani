using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberMovieListDTOs
{
    public class CreateMemberMovieListDto
    {
        [Required(ErrorMessage = "Üye ID zorunludur.")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Liste adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Liste adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }
    }
}
