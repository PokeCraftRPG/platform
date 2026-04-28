using Logitar;

namespace PokeCraft.Cms.Compiler.Settings;

internal record PokeApiSettings
{
  private const string SectionKey = "PokeApi";

  public string DataPath { get; set; } = string.Empty;

  public static PokeApiSettings Initialize(IConfiguration configuration)
  {
    PokeApiSettings settings = configuration.GetSection(SectionKey).Get<PokeApiSettings>() ?? new();

    settings.DataPath = EnvironmentHelper.GetString("POKE_API_DATA_PATH", settings.DataPath);

    return settings;
  }
}
