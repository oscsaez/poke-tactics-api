using Microsoft.Extensions.DependencyInjection;
using PokeTactics.Services.Facade;
using PokeTactics.Services.Facade.Impl;

namespace PokeTactics.Services;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPokemonService, PokemonService>();
    }
}
