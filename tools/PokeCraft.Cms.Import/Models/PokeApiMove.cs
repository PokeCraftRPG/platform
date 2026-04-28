namespace PokeCraft.Cms.Import.Models;

internal class PokeApiMove
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("type")]
  public NamedAPIResource Type { get; set; } = new();

  [JsonPropertyName("damage_class")]
  public NamedAPIResource Category { get; set; } = new();

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("flavor_text_entries")]
  public List<FlavorText> FlavorTextEntries { get; set; } = [];

  [JsonPropertyName("accuracy")]
  public int? Accuracy { get; set; }

  [JsonPropertyName("power")]
  public int? Power { get; set; }

  [JsonPropertyName("pp")]
  public int? PowerPoints { get; set; }

  public override bool Equals(object? obj) => obj is PokeApiMove move && move.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
