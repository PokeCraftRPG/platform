namespace PokeCraft.Cms.Import.Models;

internal class PokemonSpecies : INamed
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
  public NamedResource GrowthRate { get; set; } = new();

  [JsonPropertyName("hatch_counter")]
  public byte EggCycles { get; set; }

  [JsonPropertyName("egg_groups")]
  public List<NamedResource> EggGroups { get; set; } = [];

  [JsonPropertyName("pokedex_numbers")]
  public List<PokedexNumber> PokedexNumbers { get; set; } = [];

  [JsonPropertyName("genera")]
  public List<Genus> Genera { get; set; } = [];

  [JsonPropertyName("gender_rate")]
  public int GenderRatio { get; set; }

  [JsonPropertyName("forms_switchable")]
  public bool CanChangeForm { get; set; }

  [JsonPropertyName("has_gender_differences")]
  public bool HasGenderDifferences { get; set; }

  public override bool Equals(object? obj) => obj is PokemonSpecies species && species.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
