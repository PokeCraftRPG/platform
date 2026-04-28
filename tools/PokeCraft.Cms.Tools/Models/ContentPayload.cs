namespace PokeCraft.Cms.Tools.Models;

public record ContentPayload
{
  public Guid Id { get; set; }

  public ContentLocalePayload Invariant { get; set; } = new();
  public Dictionary<string, ContentLocalePayload> Locales { get; set; } = [];
}
