using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Forms.Models;

public record FormSortOption : SortOption
{
  public new FormSort Field
  {
    get => Enum.Parse<FormSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public FormSortOption(FormSort field = FormSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
