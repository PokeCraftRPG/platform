namespace PokeCraft.Cms.Import.Models;

internal record SlottedAbility
{
  [JsonPropertyName("is_hidden")]
  public bool IsHidden { get; set; }

  [JsonPropertyName("slot")]
  public int Slot { get; set; }

  [JsonPropertyName("ability")]
  public NamedResource Ability { get; set; } = new();
}
