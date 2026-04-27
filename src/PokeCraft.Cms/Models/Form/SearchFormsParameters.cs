using Krakenar.Contracts.Search;
using Krakenar.Web.Models.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Forms;
using PokeCraft.Cms.Core.Forms.Models;

namespace PokeCraft.Cms.Models.Form;

public record SearchFormsParameters : SearchParameters
{
  [FromQuery(Name = "variety")]
  public Guid? VarietyId { get; set; }

  [FromQuery(Name = "kind")]
  public FormKind? Kind { get; set; }

  [FromQuery(Name = "gender_differences")]
  public bool? HasGenderDifferences { get; set; }

  [FromQuery(Name = "type")]
  public PokemonType? Type { get; set; }

  public virtual SearchFormsPayload ToPayload()
  {
    SearchFormsPayload payload = new()
    {
      VarietyId = VarietyId,
      Kind = Kind,
      HasGenderDifferences = HasGenderDifferences,
      Type = Type
    };
    Fill(payload);

    foreach (SortOption sort in ((SearchPayload)payload).Sort)
    {
      if (Enum.TryParse(sort.Field, out FormSort field))
      {
        payload.Sort.Add(new FormSortOption(field, sort.IsDescending));
      }
    }

    return payload;
  }
}
