using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeTactics.Core.Entities;

namespace PokeTactics.Infrastructure.Data.EntityConfigurations
{
    public class AbilityInPokemonConfiguration : IEntityTypeConfiguration<AbilityInPokemon>
    {
        public void Configure(EntityTypeBuilder<AbilityInPokemon> builder)
        {
            builder.HasKey(x => new { x.PokemonId, x.AbilityId });

            builder.HasOne(x => x.Pokemon)
                .WithMany(p => p.AbilitiesInPokemon)
                .HasForeignKey(x => x.PokemonId);

            builder.HasOne(x => x.Ability)
                .WithMany(a => a.AbilitiesInPokemon)
                .HasForeignKey(x => x.AbilityId);
        }
    }
}