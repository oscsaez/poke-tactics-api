using PokeTactics.Api.HostedServices;
using PokeTactics.Api.SyncServices;
using PokeTactics.Api.Utils;
using PokeTactics.Core.Interfaces.SyncServices;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Api;

public static class ServiceCollectionExtensions
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddExternalApiHttpClient();
        services.AddHostedServices();
        services.AddSyncServices();
    }

    private static void AddExternalApiHttpClient(this IServiceCollection services)
    {
        services.AddHttpClient(ApiConstants.ExternalApiName, client =>
        {
            client.BaseAddress = ApiConstants.BasePokemonInfoUri.ToUri();
        });
    }

    private static void AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<PokemonSyncHostedService>();
    }

    private static void AddSyncServices(this IServiceCollection services)
    {
        services.AddScoped<IAbilitySyncService, AbilitySyncService>();
        services.AddScoped<IMoveSyncService, MoveSyncService>();
        services.AddScoped<IPokemonSyncService, PokemonSyncService>();
    }
}
