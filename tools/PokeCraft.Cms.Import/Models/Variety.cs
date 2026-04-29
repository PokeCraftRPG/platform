namespace PokeCraft.Cms.Import.Models;

internal class Variety
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("species")]
  public PokemonSpecies Species { get; set; } = new();

  [JsonPropertyName("is_default")]
  public bool IsDefault { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("height")]
  public int Height { get; set; }

  [JsonPropertyName("weight")]
  public int Weight { get; set; }

  [JsonPropertyName("abilities")]
  public List<SlottedAbility> Abilities { get; set; } = [];

  [JsonPropertyName("stats")]
  public List<PokemonStatistic> Statistics { get; set; } = [];

  [JsonPropertyName("base_experience")]
  public int? ExperienceYield { get; set; }

  public override bool Equals(object? obj) => obj is Variety variety && variety.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
