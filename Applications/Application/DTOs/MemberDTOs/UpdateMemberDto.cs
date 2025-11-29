using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MemberDTOs
{
    public class UpdateMemberDto
    {
        [Required(ErrorMessage = "Üye ID zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad zorunludur.")]
        [MaxLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [MaxLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
        public string Email { get; set; }

        // Şifrenin güncellenmesi isteğe bağlı, o yüzden Required koymadık
        [MaxLength(200, ErrorMessage = "Şifre en fazla 200 karakter olabilir.")]
        public string Password { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [MaxLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Üyelik planı seçimi zorunludur.")]
        public int MembershipPlanId { get; set; }

        // Plan değiştirirken MembershipStartDate güncellenebilsin diye isteğe bağlı bıraktık
        public DateTime? MembershipStartDate { get; set; } // Plan değiştirmek isterse
    }
}
