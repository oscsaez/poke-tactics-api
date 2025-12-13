using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeTactics.Core.Entities;

namespace PokeTactics.Infrastructure.Data.EntityConfigurations
{
    public class SpriteConfiguration : IEntityTypeConfiguration<Sprite>
    {
        public void Configure(EntityTypeBuilder<Sprite> builder)
        {
            builder.HasIndex(s => s.OfficialArtworkUri)
                .IsUnique();
        }
    }
}