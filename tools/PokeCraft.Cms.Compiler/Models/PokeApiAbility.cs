namespace PokeCraft.Cms.Compiler.Models;

internal class PokeApiAbility
{
  [JsonPropertyName("id")]
  public int Id { get; set; }

  [JsonPropertyName("name")]
  public string UniqueName { get; set; } = string.Empty;

  [JsonPropertyName("names")]
  public List<LocalizedName> DisplayNames { get; set; } = [];

  [JsonPropertyName("flavor_text_entries")]
  public List<FlavorText> FlavorTextEntries { get; set; } = [];

  public override bool Equals(object? obj) => obj is PokeApiAbility ability && ability.Id == Id;
  public override int GetHashCode() => Id.GetHashCode();
  public override string ToString() => $"{UniqueName} | {base.ToString()} (Id={Id})";
}
