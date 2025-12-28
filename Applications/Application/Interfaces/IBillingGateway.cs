using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBillingGateway
    {
        Task<BillingGatewayResult> ChargeAsync(int memberId, decimal amount, string period);
    }

    public class BillingGatewayResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }

        public static BillingGatewayResult Ok() => new BillingGatewayResult { Success = true };
        public static BillingGatewayResult Fail(string error) => new BillingGatewayResult { Success = false, Error = error };
    }
}
