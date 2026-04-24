using Krakenar.Contracts.Search;

namespace PokeCraft.Cms.Core.Moves.Models;

public record MoveSortOption : SortOption
{
  public new MoveSort Field
  {
    get => Enum.Parse<MoveSort>(base.Field);
    set => base.Field = value.ToString();
  }

  public MoveSortOption(MoveSort field = MoveSort.Name, bool isDescending = false)
    : base(field.ToString(), isDescending)
  {
  }
}
