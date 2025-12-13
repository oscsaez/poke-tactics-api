using Microsoft.Extensions.DependencyInjection;
using PokeTactics.Core.Interfaces;
using PokeTactics.Core.Interfaces.Daos;
using PokeTactics.Infrastructure.Daos;
using PokeTactics.Infrastructure.Data;

namespace PokeTactics.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddDataServices();
        }

        public static void AddDataServices(this IServiceCollection services)
        {
            services.AddDbContext<PokeTacticsContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddDaos(this IServiceCollection services)
        {
            services.AddScoped<IAbilityDao, AbilityDao>();
            services.AddScoped<IMoveDao, MoveDao>();
            services.AddScoped<IPokemonDao, PokemonDao>();
        }
    }
}