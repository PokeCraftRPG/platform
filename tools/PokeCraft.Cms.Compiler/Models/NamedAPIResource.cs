namespace PokeCraft.Cms.Compiler.Models;

internal record NamedAPIResource
{
  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("url")]
  public string Url { get; set; } = string.Empty;
}
