namespace PokeCraft.Cms.Import.Models;

internal record NamedResource
{
  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("url")]
  public string Url { get; set; } = string.Empty;
}
