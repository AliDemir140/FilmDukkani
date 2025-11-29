using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberMovieListDTOs
{
    public class UpdateMemberMovieListNameDto
    {
        [Required(ErrorMessage = "Liste ID zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Liste adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Liste adı en fazla 100 karakter olabilir.")]
        public string Name { get; set; }
    }
}
