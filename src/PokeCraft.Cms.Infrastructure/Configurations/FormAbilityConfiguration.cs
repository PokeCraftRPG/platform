using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PokeCraft.Cms.Core.Abilities;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Configurations;

internal class FormAbilityConfiguration : IEntityTypeConfiguration<FormAbilityEntity>
{
  public void Configure(EntityTypeBuilder<FormAbilityEntity> builder)
  {
    builder.ToTable(nameof(PokemonContext.FormAbilities), PokemonContext.Schema);
    builder.HasKey(x => new { x.FormId, x.AbilityId });

    builder.HasIndex(x => x.AbilityId);
    builder.HasIndex(x => x.Slot);

    builder.Property(x => x.Slot).HasMaxLength(16).HasConversion(new EnumToStringConverter<AbilitySlot>());

    builder.HasOne(x => x.Form).WithMany(x => x.Abilities).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.Ability).WithMany(x => x.Forms).OnDelete(DeleteBehavior.Cascade);
  }
}
