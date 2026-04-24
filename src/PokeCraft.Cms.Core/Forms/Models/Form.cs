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

  public PokemonType PrimaryType { get; set; }
  public PokemonType? SecondaryType { get; set; }

  // TODO(fpion): Abilities
  // TODO(fpion): BaseStatistics
  public int YieldExperience { get; set; }
  // TODO(fpion): Yield

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
