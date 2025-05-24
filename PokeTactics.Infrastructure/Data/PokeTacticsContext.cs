using Microsoft.EntityFrameworkCore;

namespace PokeTactics.Infrastructure.Data
{
    public class PokeTacticsContext : DbContext
    {
        public PokeTacticsContext(DbContextOptions<PokeTacticsContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PokeTacticsContext).Assembly);
        }
    }
}