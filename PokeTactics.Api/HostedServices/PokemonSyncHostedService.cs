
using PokeTactics.Api.Utils;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Api.HostedServices
{
    public class PokemonSyncHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<PokemonSyncHostedService> _logger;
        private readonly HttpClient _httpClient;

        public PokemonSyncHostedService(IServiceProvider serviceProvider, ILogger<PokemonSyncHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = ApiConstants.AllPokemonInfoUri.ToUri() };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Pokemon synchronization service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // TODO Call SyncIfNeeded...
            }
        }

        private async Task SyncIfNeeded(CancellationToken cancellationToken)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            // TODO Finish versions comparison and pokemon update
        }
    }
}