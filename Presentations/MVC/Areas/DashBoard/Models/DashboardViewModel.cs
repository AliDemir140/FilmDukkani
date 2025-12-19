using Application.DTOs.DeliveryRequestDTOs;
using System.Collections.Generic;

namespace MVC.Areas.DashBoard.Models
{
    public class DashboardViewModel
    {
        public int TotalMovies { get; set; }
        public int TotalCategories { get; set; }
        public int TotalMembers { get; set; }

        public int PendingDeliveries { get; set; }
        public int PreparedDeliveries { get; set; }
        public int ShippedDeliveries { get; set; }
        public int DeliveredDeliveries { get; set; }

        public int DamagedCopies { get; set; }

        public List<DeliveryRequestDto> LastRequests { get; set; } = new();
    }
}
