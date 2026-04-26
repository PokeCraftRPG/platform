using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Moves.Models;

namespace PokeCraft.Cms.Core.Varieties.Models;

public record VarietyMove
{
  public Move Move { get; set; } = new();

  public LearningMethod Method { get; set; }
  public int? Level { get; set; }
}
