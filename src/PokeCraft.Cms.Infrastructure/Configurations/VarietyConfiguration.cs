using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Configurations;

internal class VarietyConfiguration : AggregateConfiguration<VarietyEntity>, IEntityTypeConfiguration<VarietyEntity>
{
  public override void Configure(EntityTypeBuilder<VarietyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PokemonContext.Varieties), PokemonContext.Schema);
    builder.HasKey(x => x.VarietyId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.IsPublished);
    builder.HasIndex(x => x.SpeciesId);
    builder.HasIndex(x => x.IsDefault);
    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.Genus);
    builder.HasIndex(x => x.CanChangeForm);
    builder.HasIndex(x => x.GenderRatio);

    builder.Property(x => x.Key).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Genus).HasMaxLength(16);

    builder.HasOne(x => x.Species).WithMany(x => x.Varieties).OnDelete(DeleteBehavior.Restrict);
  }
}
