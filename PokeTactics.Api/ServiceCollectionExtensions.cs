using PokeTactics.Api.HostedServices;
using PokeTactics.Api.Utils;
using PokeTactics.Core.Utils.Extensions;

namespace PokeTactics.Api;

public static class ServiceCollectionExtensions
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddExternalApiHttpClient();
        services.AddHostedServices();
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
}
