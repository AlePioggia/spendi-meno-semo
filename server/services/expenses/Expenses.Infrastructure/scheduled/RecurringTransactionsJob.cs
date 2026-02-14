using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Expenses.Infrastructure.scheduled
{
    public class RecurringTransactionsJob : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RecurringTransactionsJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<RecurringTransactionsJobRunner>();
                await runner.RunOnceAsync(stoppingToken);
            }
        }
    }
}
