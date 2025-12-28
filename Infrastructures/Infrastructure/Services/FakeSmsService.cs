using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class FakeSmsService : ISmsService
    {
        private readonly ILogger<FakeSmsService> _logger;

        public FakeSmsService(ILogger<FakeSmsService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string phone, string message)
        {
            _logger.LogInformation("SMS To: {Phone}", phone);
            _logger.LogInformation("Message: {Message}", message);
            return Task.CompletedTask;
        }
    }
}
