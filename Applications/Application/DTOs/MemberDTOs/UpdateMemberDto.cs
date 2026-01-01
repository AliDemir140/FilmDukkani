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
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [MaxLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [MaxLength(100, ErrorMessage = "Email en fazla 100 karakter olabilir.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(200, ErrorMessage = "Şifre en fazla 200 karakter olabilir.")]
        public string? Password { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [MaxLength(20, ErrorMessage = "Telefon en fazla 20 karakter olabilir.")]
        public string? Phone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Lütfen bir üyelik planı seçiniz.")]
        public int MembershipPlanId { get; set; }

        public DateTime? MembershipStartDate { get; set; }

        public string IdentityUserId { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;
    }
}
