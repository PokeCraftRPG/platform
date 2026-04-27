using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Configurations;

internal class SpeciesConfiguration : AggregateConfiguration<SpeciesEntity>, IEntityTypeConfiguration<SpeciesEntity>
{
  public override void Configure(EntityTypeBuilder<SpeciesEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PokemonContext.Species), PokemonContext.Schema);
    builder.HasKey(x => x.SpeciesId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.IsPublished);
    builder.HasIndex(x => x.Number).IsUnique();
    builder.HasIndex(x => x.Category);
    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.BaseFriendship);
    builder.HasIndex(x => x.CatchRate);
    builder.HasIndex(x => x.GrowthRate);
    builder.HasIndex(x => x.EggCycles);
    builder.HasIndex(x => x.PrimaryEggGroup);
    builder.HasIndex(x => x.SecondaryEggGroup);

    builder.Property(x => x.Category).HasMaxLength(16).HasConversion(new EnumToStringConverter<SpeciesCategory>());
    builder.Property(x => x.Key).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.GrowthRate).HasMaxLength(16).HasConversion(new EnumToStringConverter<GrowthRate>());
    builder.Property(x => x.PrimaryEggGroup).HasMaxLength(16).HasConversion(new EnumToStringConverter<EggGroup>());
    builder.Property(x => x.SecondaryEggGroup).HasMaxLength(16).HasConversion(new EnumToStringConverter<EggGroup>());
  }
}
