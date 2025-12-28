using Application.Repositories;
using Application.ServiceManager;
using Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class MonthlyBillingHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MonthlyBillingHostedService> _logger;

        public MonthlyBillingHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<MonthlyBillingHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var intervalMinutes = _configuration.GetValue<int>("Billing:HostedServiceIntervalMinutes", 60);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunOnceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "MonthlyBillingHostedService ExecuteAsync error");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private async Task RunOnceAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var memberRepository = scope.ServiceProvider.GetRequiredService<IMemberRepository>();
            var billingService = scope.ServiceProvider.GetRequiredService<BillingServiceManager>();

            var adminEmail =
                _configuration["Billing:AdminEmail"]
                ?? _configuration["SeedAdmin:Email"]
                ?? string.Empty;

            var period = DateTime.Today.ToString("yyyy-MM");
            var day = DateTime.Today.Day;

            var members = await memberRepository.GetAllAsync(m =>
                m.Status != MemberStatus.Deleted &&
                m.MembershipStartDate.Day == day
            );

            if (members == null || members.Count == 0)
                return;

            foreach (var m in members.OrderBy(x => x.ID))
            {
                if (stoppingToken.IsCancellationRequested)
                    break;

                try
                {
                    await billingService.RunForMemberAsync(m.ID, period, adminEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Billing run failed for MemberId={MemberId}", m.ID);
                }
            }
        }
    }
}
