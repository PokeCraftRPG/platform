using Krakenar.Contracts;
using PokeCraft.Cms.Core.Forms.Models;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Core.Varieties.Models;

public class Variety : Aggregate
{
  public PokemonSpecies Species { get; set; } = new();
  public bool IsDefault { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Genus { get; set; }
  public string? Description { get; set; }

  public byte? GenderRatio { get; set; }

  public bool CanChangeForm { get; set; }
  public List<Form> Forms { get; set; } = [];

  public List<VarietyMove> Moves { get; set; } = [];

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
