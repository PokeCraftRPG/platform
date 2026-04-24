using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Forms.Models;

public record SearchFormsPayload : SearchPayload
{
  public Guid? VarietyId { get; set; }
  public bool? IsDefault { get; set; }
  public bool? HasGenderDifferences { get; set; }
  public bool? IsBattleOnly { get; set; }
  public bool? IsMega { get; set; }
  public PokemonType? Type { get; set; }

  public new List<FormSortOption> Sort { get; set; } = [];
}
