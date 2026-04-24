namespace PokeCraft.Cms.Core.Forms.Models;

public record FormTypes
{
  public PokemonType Primary { get; set; }
  public PokemonType? Secondary { get; set; }
}
