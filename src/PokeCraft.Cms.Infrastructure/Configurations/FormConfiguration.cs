using Krakenar.Core;
using Krakenar.EntityFrameworkCore.Relational.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Forms;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Configurations;

internal class FormConfiguration : AggregateConfiguration<FormEntity>, IEntityTypeConfiguration<FormEntity>
{
  public override void Configure(EntityTypeBuilder<FormEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PokemonContext.Forms), PokemonContext.Schema);
    builder.HasKey(x => x.FormId);

    builder.HasIndex(x => x.UniqueId).IsUnique();
    builder.HasIndex(x => x.IsPublished);
    builder.HasIndex(x => x.VarietyId);
    builder.HasIndex(x => x.Kind);
    builder.HasIndex(x => x.Key).IsUnique();
    builder.HasIndex(x => x.Name);
    builder.HasIndex(x => x.HasGenderDifferences);
    builder.HasIndex(x => x.Height);
    builder.HasIndex(x => x.Weight);
    builder.HasIndex(x => x.PrimaryType);
    builder.HasIndex(x => x.SecondaryType);
    builder.HasIndex(x => x.YieldExperience);

    builder.Property(x => x.Kind).HasMaxLength(16).HasConversion(new EnumToStringConverter<FormKind>());
    builder.Property(x => x.Key).HasMaxLength(UniqueName.MaximumLength);
    builder.Property(x => x.Name).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.PrimaryType).HasMaxLength(16).HasConversion(new EnumToStringConverter<PokemonType>());
    builder.Property(x => x.SecondaryType).HasMaxLength(16).HasConversion(new EnumToStringConverter<PokemonType>());

    builder.HasOne(x => x.Variety).WithMany(x => x.Forms).OnDelete(DeleteBehavior.Restrict);
  }
}
