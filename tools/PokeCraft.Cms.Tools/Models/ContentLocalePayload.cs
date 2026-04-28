namespace PokeCraft.Cms.Tools.Models;

public record ContentLocalePayload
{
  public bool IsPublished { get; set; }
  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
  public Dictionary<string, string> FieldValues { get; set; } = [];
}
