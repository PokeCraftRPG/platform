namespace PokeCraft.Cms.Compiler.Models;

internal record FlavorText
{
  [JsonPropertyName("flavor_text")]
  public string Value { get; set; } = string.Empty;

  [JsonPropertyName("language")]
  public NamedAPIResource Language { get; set; } = new();

  [JsonPropertyName("version_group")]
  public NamedAPIResource VersionGroup { get; set; } = new();
}
