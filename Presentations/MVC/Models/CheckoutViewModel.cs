using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC.Models
{
    public class CheckoutViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();

        public List<SelectListItem> MemberLists { get; set; } = new();

        public int SelectedListId { get; set; }

        public DateTime DeliveryDate { get; set; }
    }
}
