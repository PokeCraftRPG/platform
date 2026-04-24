using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Varieties.Models;

public record VarietySortOption : SortOption
{
  public new VarietySort Field
  {
    get => Enum.Parse<VarietySort>(base.Field);
    set => base.Field = value.ToString();
  }

  public VarietySortOption(VarietySort field = VarietySort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
