namespace PokeCraft.Cms.Import.Models;

internal class PokeApiVariety
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("species")]
  public NamedAPIResource Species { get; set; } = new();

  [JsonPropertyName("is_default")]
  public bool IsDefault { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  // TODO(fpion): GenderRatio → species.gender_rate

  // TODO(fpion): CanChangeForm → species.forms_switchable

  // TODO(fpion): Moves

  // TODO(fpion): Genus → species.genera

  public override bool Equals(object? obj) => obj is PokeApiVariety variety && variety.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
