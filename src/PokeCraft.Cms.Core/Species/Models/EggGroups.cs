namespace PokeCraft.Cms.Core.Species.Models;

public record EggGroups
{
  public EggGroup Primary { get; set; }
  public EggGroup? Secondary { get; set; }
}
