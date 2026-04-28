namespace PokeCraft.Cms.Compiler.Models;

internal record PokedexNumber
{
  [JsonPropertyName("entry_number")]
  public int Number { get; set; }

  [JsonPropertyName("pokedex")]
  public NamedAPIResource Pokedex { get; set; } = new();
}
