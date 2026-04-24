namespace PokeCraft.Cms.Infrastructure;

internal static class PokemonHelper
{
  public static string Normalize(string value) => value.Trim().ToLowerInvariant();
}
