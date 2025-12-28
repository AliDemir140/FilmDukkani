using System.Collections.Generic;

namespace Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<DeliveryRequest> DeliveryRequests { get; set; } = new List<DeliveryRequest>();
    }
}
