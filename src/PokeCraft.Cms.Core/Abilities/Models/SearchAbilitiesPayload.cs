using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Abilities.Models;

public record SearchAbilitiesPayload : SearchPayload
{
  public new List<AbilitySortOption> Sort { get; set; } = [];
}
