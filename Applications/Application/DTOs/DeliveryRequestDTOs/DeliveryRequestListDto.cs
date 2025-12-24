using Domain.Enums;

namespace Application.DTOs.DeliveryRequestDTOs
{
    public class DeliveryRequestListDto
    {
        public int Id { get; set; }
        public string ListName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DeliveryStatus Status { get; set; }
    }
}
