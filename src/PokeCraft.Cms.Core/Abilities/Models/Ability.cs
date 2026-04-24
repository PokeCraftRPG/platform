using Krakenar.Contracts;

namespace PokeCraft.Cms.Core.Abilities.Models;

public class Ability : Aggregate
{
  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
