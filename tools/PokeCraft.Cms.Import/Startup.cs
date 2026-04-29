using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Import.Tasks;

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
    services.AddSingleton<ImportAbilitiesTask>();
    services.AddSingleton<ImportFormsTask>();
    services.AddSingleton<ImportMovesTask>();
    services.AddSingleton<ImportSpeciesTask>();
    services.AddSingleton<ImportVarietiesTask>();
  }
}
