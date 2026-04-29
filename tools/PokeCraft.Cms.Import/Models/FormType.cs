namespace PokeCraft.Cms.Import.Models;

internal record FormType
{
  [JsonPropertyName("slot")]
  public int Slot { get; set; }

  [JsonPropertyName("type")]
  public NamedResource Type { get; set; } = new();
}
