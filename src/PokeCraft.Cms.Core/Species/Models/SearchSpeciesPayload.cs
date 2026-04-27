using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Species.Models;

public record SearchSpeciesPayload : SearchPayload
{
  public SpeciesCategory? Category { get; set; }
  public GrowthRate? GrowthRate { get; set; }
  public EggGroup? EggGroup { get; set; }

  public new List<SpeciesSortOption> Sort { get; set; } = [];
}
