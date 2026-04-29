namespace PokeCraft.Cms.Import.Models;

internal static class PokeApiExtensions
{
  public static string? GetDescription(this IDescribed described, string language, string versionGroup)
  {
    FlavorText[] flavorTexts = described.Descriptions.Where(x => x.Language.UniqueName == language && x.VersionGroup.UniqueName == versionGroup).ToArray();
    return flavorTexts.Length == 1 ? flavorTexts[0].Value : null;
  }

  public static string? GetDisplayName(this INamed named, string language)
  {
    LocalizedName[] displayNames = named.DisplayNames.Where(x => x.Language.UniqueName == language).ToArray();
    return displayNames.Length == 1 ? displayNames[0].Value : null;
  }
}
