using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeTactics.Core.Entities;

namespace PokeTactics.Infrastructure.Data.EntityConfigurations
{
    public class MoveInPokemonConfiguration : IEntityTypeConfiguration<MoveInPokemon>
    {
        public void Configure(EntityTypeBuilder<MoveInPokemon> builder)
        {
            builder.HasKey(x => new { x.PokemonId, x.MoveId });

            builder.HasOne(x => x.Pokemon)
                .WithMany(p => p.MovesInPokemon)
                .HasForeignKey(x => x.PokemonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Move)
                .WithMany(m => m.MovesInPokemon)
                .HasForeignKey(x => x.MoveId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}