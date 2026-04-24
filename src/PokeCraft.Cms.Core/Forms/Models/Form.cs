using Krakenar.Contracts;
using PokeCraft.Cms.Core.Varieties.Models;

namespace PokeCraft.Cms.Core.Forms.Models;

public class Form : Aggregate
{
  public Variety Variety { get; set; } = new();
  public bool IsDefault { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public bool HasGenderDifferences { get; set; }
  public bool IsBattleOnly { get; set; }
  public bool IsMega { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public FormTypes Types { get; set; } = new();
  public FormAbilities Abilities { get; set; } = new();
  public BaseStatistics BaseStatistics { get; set; } = new();
  public Yield Yield { get; set; } = new();

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
