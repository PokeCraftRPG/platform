namespace PokeCraft.Cms.Compiler.Models;

internal class PokeApiSpecies
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("is_baby")]
  public bool IsBaby { get; set; }

  [JsonPropertyName("is_legendary")]
  public bool IsLegendary { get; set; }

  [JsonPropertyName("is_mythical")]
  public bool IsMythical { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("base_happiness")]
  public byte BaseFriendship { get; set; }

  [JsonPropertyName("capture_rate")]
  public byte CatchRate { get; set; }

  [JsonPropertyName("growth_rate")]
  public NamedAPIResource GrowthRate { get; set; } = new();

  [JsonPropertyName("hatch_counter")]
  public byte EggCycles { get; set; }

  [JsonPropertyName("egg_groups")]
  public List<NamedAPIResource> EggGroups { get; set; } = [];

  [JsonPropertyName("pokedex_numbers")]
  public List<PokedexNumber> PokedexNumbers { get; set; } = [];

  public override bool Equals(object? obj) => obj is PokeApiSpecies species && species.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
