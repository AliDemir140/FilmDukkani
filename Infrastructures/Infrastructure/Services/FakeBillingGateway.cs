using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services
{
    public class FakeBillingGateway : IBillingGateway
    {
        private readonly IConfiguration _configuration;
        private readonly Random _rnd = new Random();

        public FakeBillingGateway(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<BillingGatewayResult> ChargeAsync(int memberId, decimal amount, string period)
        {
            var failRate = _configuration.GetValue<int>("Billing:SimulateFailRatePercent", 0);
            if (failRate <= 0)
                return Task.FromResult(BillingGatewayResult.Ok());

            var roll = _rnd.Next(1, 101);
            if (roll <= failRate)
                return Task.FromResult(BillingGatewayResult.Fail("Simulated payment failure"));

            return Task.FromResult(BillingGatewayResult.Ok());
        }
    }
}
