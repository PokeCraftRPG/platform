namespace PokeCraft.Cms.Import.Models;

internal record LocalizedName
{
  [JsonPropertyName("language")]
  public NamedAPIResource Language { get; set; } = new();

  [JsonPropertyName("name")]
  public string Value { get; set; } = string.Empty;
}
