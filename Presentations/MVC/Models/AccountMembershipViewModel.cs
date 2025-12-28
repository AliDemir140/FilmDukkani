using Application.DTOs.MemberDTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Models
{
    public class AccountMembershipViewModel
    {
        public MemberProfileDto? Profile { get; set; }

        public List<SelectListItem> Plans { get; set; } = new List<SelectListItem>();

        public int SelectedPlanId { get; set; }

        public string StatusText { get; set; } = string.Empty;

        public bool IsBlockedForCheckout { get; set; }

        public bool IsActive { get; set; }
    }
}
