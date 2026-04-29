namespace PokeCraft.Cms.Import.Models;

internal class Form : INamed
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("pokemon")]
  public NamedResource Variety { get; set; } = new();

  [JsonPropertyName("is_default")]
  public bool IsDefault { get; set; }

  [JsonPropertyName("is_battle_only")]
  public bool IsBattleOnly { get; set; }

  [JsonPropertyName("is_mega")]
  public bool IsMega { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("types")]
  public List<FormType> Types { get; set; } = [];

  public override bool Equals(object? obj) => obj is Form form && form.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
