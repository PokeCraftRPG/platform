using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Models.Species;

public record SearchSpeciesParameters : SearchParameters
{
  [FromQuery(Name = "category")]
  public PokemonCategory? Category { get; set; }

  [FromQuery(Name = "growth_rate")]
  public GrowthRate? GrowthRate { get; set; }

  [FromQuery(Name = "egg_group")]
  public EggGroup? EggGroup { get; set; }

  public virtual SearchSpeciesPayload ToPayload()
  {
    SearchSpeciesPayload payload = new()
    {
      Category = Category,
      GrowthRate = GrowthRate,
      EggGroup = EggGroup
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out SpeciesSort field))
      {
        payload.Sort.Add(new SpeciesSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
