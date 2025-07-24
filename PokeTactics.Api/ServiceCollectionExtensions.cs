using PokeTactics.Api.HostedServices;

namespace PokeTactics.Api;

public static class ServiceCollectionExtensions
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddHostedServices();
    }

    private static void AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<PokemonSyncHostedService>();
    }
}
