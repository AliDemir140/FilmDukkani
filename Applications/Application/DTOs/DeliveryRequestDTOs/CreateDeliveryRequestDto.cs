using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.DeliveryRequestDTOs
{
    public class CreateDeliveryRequestDto
    {
        [Required(ErrorMessage = "Üye bilgisi zorunludur.")]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Film listesi zorunludur.")]
        public int MemberMovieListId { get; set; }

        [Required(ErrorMessage = "Teslimat tarihi zorunludur.")]
        public DateTime DeliveryDate { get; set; }
    }
}
