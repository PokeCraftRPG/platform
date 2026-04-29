namespace PokeCraft.Cms.Import.Models;

internal record FlavorText
{
  [JsonPropertyName("flavor_text")]
  public string Value { get; set; } = string.Empty;

  [JsonPropertyName("language")]
  public NamedResource Language { get; set; } = new();

  [JsonPropertyName("version_group")]
  public NamedResource VersionGroup { get; set; } = new();
}
