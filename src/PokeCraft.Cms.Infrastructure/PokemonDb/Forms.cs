using Logitar.Data;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.PokemonDb;

internal static class Forms
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.Forms), alias: null);

  public static readonly ColumnId CreatedBy = new(nameof(FormEntity.CreatedBy), Table);
  public static readonly ColumnId CreatedOn = new(nameof(FormEntity.CreatedOn), Table);
  public static readonly ColumnId StreamId = new(nameof(FormEntity.StreamId), Table);
  public static readonly ColumnId UpdatedBy = new(nameof(FormEntity.UpdatedBy), Table);
  public static readonly ColumnId UpdatedOn = new(nameof(FormEntity.UpdatedOn), Table);
  public static readonly ColumnId Version = new(nameof(FormEntity.Version), Table);

  public static readonly ColumnId BaseAttack = new(nameof(FormEntity.BaseAttack), Table);
  public static readonly ColumnId BaseDefense = new(nameof(FormEntity.BaseDefense), Table);
  public static readonly ColumnId BaseHP = new(nameof(FormEntity.BaseHP), Table);
  public static readonly ColumnId BaseSpecialAttack = new(nameof(FormEntity.BaseSpecialAttack), Table);
  public static readonly ColumnId BaseSpecialDefense = new(nameof(FormEntity.BaseSpecialDefense), Table);
  public static readonly ColumnId BaseSpeed = new(nameof(FormEntity.BaseSpeed), Table);
  public static readonly ColumnId Description = new(nameof(FormEntity.Description), Table);
  public static readonly ColumnId FormId = new(nameof(FormEntity.FormId), Table);
  public static readonly ColumnId HasGenderDifferences = new(nameof(FormEntity.HasGenderDifferences), Table);
  public static readonly ColumnId Height = new(nameof(FormEntity.Height), Table);
  public static readonly ColumnId IsPublished = new(nameof(FormEntity.IsPublished), Table);
  public static readonly ColumnId Key = new(nameof(FormEntity.Key), Table);
  public static readonly ColumnId Kind = new(nameof(FormEntity.Kind), Table);
  public static readonly ColumnId Name = new(nameof(FormEntity.Name), Table);
  public static readonly ColumnId PrimaryType = new(nameof(FormEntity.PrimaryType), Table);
  public static readonly ColumnId SecondaryType = new(nameof(FormEntity.SecondaryType), Table);
  public static readonly ColumnId UniqueId = new(nameof(FormEntity.UniqueId), Table);
  public static readonly ColumnId VarietyId = new(nameof(FormEntity.VarietyId), Table);
  public static readonly ColumnId Weight = new(nameof(FormEntity.Weight), Table);
  public static readonly ColumnId YieldAttack = new(nameof(FormEntity.YieldAttack), Table);
  public static readonly ColumnId YieldDefense = new(nameof(FormEntity.YieldDefense), Table);
  public static readonly ColumnId YieldExperience = new(nameof(FormEntity.YieldExperience), Table);
  public static readonly ColumnId YieldHP = new(nameof(FormEntity.YieldHP), Table);
  public static readonly ColumnId YieldSpecialAttack = new(nameof(FormEntity.YieldSpecialAttack), Table);
  public static readonly ColumnId YieldSpecialDefense = new(nameof(FormEntity.YieldSpecialDefense), Table);
  public static readonly ColumnId YieldSpeed = new(nameof(FormEntity.YieldSpeed), Table);
}
