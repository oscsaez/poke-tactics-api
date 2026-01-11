using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeTactics.Core.Entities;

namespace PokeTactics.Infrastructure.Data.EntityConfigurations
{
    public class StatConfiguration : IEntityTypeConfiguration<Stat>
    {
        public void Configure(EntityTypeBuilder<Stat> builder)
        {
            builder.HasOne(s => s.Pokemon)
                .WithMany(p => p.Stats)
                .HasForeignKey(s => s.PokemonId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}