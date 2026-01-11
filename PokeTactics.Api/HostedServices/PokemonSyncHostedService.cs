using Microsoft.Extensions.Options;
using PokeTactics.Core.Definitions;
using PokeTactics.Core.Interfaces.SyncServices;

namespace PokeTactics.Api.HostedServices
{
    public class PokemonSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PokemonSyncHostedService> _logger;
        private readonly int _intervalMinutes;

        public PokemonSyncHostedService(
            IServiceProvider serviceProvider,
            ILogger<PokemonSyncHostedService> logger,
            IOptions<PokemonSyncSettings> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _intervalMinutes = options.Value.IntervalMinutes;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pokemon synchronization service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncIfNeeded(stoppingToken);

                    // Wait until next synchronization
                    await Task.Delay(TimeSpan.FromMinutes(_intervalMinutes), stoppingToken);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Error while synchronizing Pokemon.");
                }
            }
        }

        private async Task SyncIfNeeded(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ISyncService syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

            await syncService.Sync(cancellationToken);
        }
    }
}