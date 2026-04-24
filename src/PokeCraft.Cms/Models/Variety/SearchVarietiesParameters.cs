using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Varieties.Models;

namespace PokeCraft.Cms.Models.Variety;

public record SearchVarietiesParameters : SearchParameters
{
  [FromQuery(Name = "species")]
  public Guid? SpeciesId { get; set; }

  [FromQuery(Name = "default")]
  public bool? IsDefault { get; set; }

  [FromQuery(Name = "metamorph")]
  public bool? CanChangeForm { get; set; }

  public virtual SearchVarietiesPayload ToPayload()
  {
    SearchVarietiesPayload payload = new()
    {
      SpeciesId = SpeciesId,
      IsDefault = IsDefault,
      CanChangeForm = CanChangeForm
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out VarietySort field))
      {
        payload.Sort.Add(new VarietySortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
