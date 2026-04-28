namespace PokeCraft.Cms.Infrastructure;

public static class PokemonHelper
{
  public static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
