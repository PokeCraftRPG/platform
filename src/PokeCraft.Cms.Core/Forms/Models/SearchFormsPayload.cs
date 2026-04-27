using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Forms.Models;

public record SearchFormsPayload : SearchPayload
{
  public Guid? VarietyId { get; set; }
  public FormKind? Kind { get; set; }
  public bool? HasGenderDifferences { get; set; }
  public PokemonType? Type { get; set; }

  public new List<FormSortOption> Sort { get; set; } = [];
}
