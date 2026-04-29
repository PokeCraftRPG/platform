namespace PokeCraft.Cms.Import.Models;

internal record PokemonStatistic
{
  [JsonPropertyName("stat")]
  public NamedResource Statistic { get; set; } = new();

  [JsonPropertyName("base_stat")]
  public byte Base { get; set; }

  [JsonPropertyName("effort")]
  public byte Yield { get; set; }
}
