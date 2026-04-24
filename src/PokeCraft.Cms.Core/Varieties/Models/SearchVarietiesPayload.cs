using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Varieties.Models;

public record SearchVarietiesPayload : SearchPayload
{
  public Guid? SpeciesId { get; set; }
  public bool? IsDefault { get; set; }
  public bool? CanChangeForm { get; set; }

  public new List<VarietySortOption> Sort { get; set; } = [];
}
