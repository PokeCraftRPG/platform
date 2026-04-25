using PokeCraft.Cms.Core.Moves;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class VarietyMoveEntity
{
  public VarietyEntity? Variety { get; private set; }
  public int VarietyId { get; private set; }

  public MoveEntity? Move { get; private set; }
  public int MoveId { get; private set; }

  public LearningMethod Method { get; set; }
  public int? Level { get; set; }

  public VarietyMoveEntity(VarietyEntity variety, MoveEntity move, LearningMethod method, int? level)
  {
    Variety = variety;
    VarietyId = variety.VarietyId;

    Move = move;
    MoveId = move.MoveId;

    Method = method;
    Level = level;
  }

  private VarietyMoveEntity()
  {
  }

  public override bool Equals(object? obj) => obj is VarietyMoveEntity entity && entity.VarietyId == VarietyId && entity.MoveId == MoveId;
  public override int GetHashCode() => HashCode.Combine(VarietyId, MoveId);
  public override string ToString() => $"{base.ToString()} (VarietyId={VarietyId}, MoveId={MoveId})";
}
