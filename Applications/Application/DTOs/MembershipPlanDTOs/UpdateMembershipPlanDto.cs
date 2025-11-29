using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.MembershipPlanDTOs
{
    public class UpdateMembershipPlanDto
    {
        [Required(ErrorMessage = "Plan ID zorunludur.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Plan adı zorunludur.")]
        [MaxLength(100, ErrorMessage = "Plan adı en fazla 100 karakter olabilir.")]
        public string PlanName { get; set; }

        [Range(0.01, 999999, ErrorMessage = "Fiyat 0'dan büyük olmalıdır.")]
        public decimal Price { get; set; }

        [Range(1, 1000, ErrorMessage = "Aylık film sayısı 1 ile 1000 arasında olmalıdır.")]
        public int MaxMoviesPerMonth { get; set; }

        [Range(1, 1000, ErrorMessage = "Aylık değişim sayısı 1 ile 1000 arasında olmalıdır.")]
        public int MaxChangePerMonth { get; set; }

        [MaxLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        public string? Description { get; set; }
    }
}
