using PokeCraft.Cms.Import.Settings;

namespace PokeCraft.Cms.Import;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<ImportWorker>();
    services.AddSingleton(PokeApiSettings.Initialize(_configuration));
  }
}
