using Microsoft.Extensions.DependencyInjection;
using PokeTactics.Core.Interfaces;
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
            // Fill with DAOs registration
        }
    }
}