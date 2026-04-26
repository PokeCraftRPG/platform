using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Configurations;

internal class VarietyMoveConfiguration : IEntityTypeConfiguration<VarietyMoveEntity>
{
  public void Configure(EntityTypeBuilder<VarietyMoveEntity> builder)
  {
    builder.ToTable(nameof(PokemonContext.VarietyMoves), PokemonContext.Schema);
    builder.HasKey(x => new { x.VarietyId, x.MoveId });

    builder.HasIndex(x => x.MoveId);
    builder.HasIndex(x => x.Method);
    builder.HasIndex(x => x.Level);

    builder.Property(x => x.Method).HasMaxLength(16).HasConversion(new EnumToStringConverter<LearningMethod>());

    builder.HasOne(x => x.Variety).WithMany(x => x.Moves).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Move).WithMany(x => x.Varieties).OnDelete(DeleteBehavior.Cascade);
  }
}
