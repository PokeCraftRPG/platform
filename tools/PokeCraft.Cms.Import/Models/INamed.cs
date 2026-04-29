namespace PokeCraft.Cms.Import.Models;

internal interface INamed
{
  string UniqueName { get; }
  List<LocalizedName> DisplayNames { get; }
}
