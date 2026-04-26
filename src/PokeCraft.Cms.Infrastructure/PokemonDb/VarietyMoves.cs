using Logitar.Data;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.PokemonDb;

internal static class VarietyMoves
{
  public static readonly TableId Table = new(PokemonContext.Schema, nameof(PokemonContext.VarietyMoves), alias: null);

  public static readonly ColumnId Level = new(nameof(VarietyMoveEntity.Level), Table);
  public static readonly ColumnId Method = new(nameof(VarietyMoveEntity.Method), Table);
  public static readonly ColumnId MoveId = new(nameof(VarietyMoveEntity.MoveId), Table);
  public static readonly ColumnId VarietyId = new(nameof(VarietyMoveEntity.VarietyId), Table);
}
