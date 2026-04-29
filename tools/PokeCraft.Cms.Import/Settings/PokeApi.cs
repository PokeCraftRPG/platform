using Logitar;

namespace PokeCraft.Cms.Import.Settings;

internal class PokeApiSettings
{
  private const string SectionKey = "PokeApi";

  public string DataDirectory { get; set; } = string.Empty;

  public static PokeApiSettings Initialize(IConfiguration configuration)
  {
    PokeApiSettings settings = configuration.GetSection(SectionKey).Get<PokeApiSettings>() ?? new();

    settings.DataDirectory = EnvironmentHelper.GetString("POKE_API_DATA_DIRECTORY", settings.DataDirectory);

    return settings;
  }
}
