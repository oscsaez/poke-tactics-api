using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeTactics.Core.Entities;

namespace PokeTactics.Infrastructure.Data.EntityConfigurations
{
    public class MoveConfiguration : IEntityTypeConfiguration<Move>
    {
        public void Configure(EntityTypeBuilder<Move> builder)
        {
            builder.HasIndex(m => m.Name)
                .IsUnique();
        }
    }
}