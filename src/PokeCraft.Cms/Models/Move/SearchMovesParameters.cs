using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Moves.Models;

namespace PokeCraft.Cms.Models.Move;

public record SearchMovesParameters : SearchParameters
{
  [FromQuery(Name = "type")]
  public PokemonType? Type { get; set; }

  [FromQuery(Name = "category")]
  public MoveCategory? Category { get; set; }

  public virtual SearchMovesPayload ToPayload()
  {
    SearchMovesPayload payload = new()
    {
      Type = Type,
      Category = Category
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out MoveSort field))
      {
        payload.Sort.Add(new MoveSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
