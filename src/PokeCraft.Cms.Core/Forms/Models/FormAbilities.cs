using PokeCraft.Cms.Core.Abilities.Models;

namespace PokeCraft.Cms.Core.Forms.Models;

public record FormAbilities
{
  public Ability Primary { get; set; } = new();
  public Ability? Secondary { get; set; }
  public Ability? Hidden { get; set; }
}
