using Logitar.Data;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.PokemonDb;

internal static class Abilities
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Abilities), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(AbilityEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(AbilityEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(AbilityEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(AbilityEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(AbilityEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(AbilityEntity.Version), Table);

  public static readonly ColumnId AbilityId = new(nameof(AbilityEntity.AbilityId), Table);
  public static readonly ColumnId Description = new(nameof(AbilityEntity.Description), Table);
  public static readonly ColumnId IsPublished = new(nameof(AbilityEntity.IsPublished), Table);
  public static readonly ColumnId Key = new(nameof(AbilityEntity.Key), Table);
  public static readonly ColumnId Name = new(nameof(AbilityEntity.Name), Table);
  public static readonly ColumnId UniqueId = new(nameof(AbilityEntity.UniqueId), Table);
}
