using Logitar.CQRS;
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
    services.AddLogitarCQRS();
    services.AddSingleton(PokeApiSettings.Initialize(_configuration));

    services.AddTransient<ICommandHandler<ImportAbilitiesTask, Unit>, ImportAbilitiesTaskHandler>();
    services.AddTransient<ICommandHandler<ImportMovesTask, Unit>, ImportMovesTaskHandler>();
    services.AddTransient<ICommandHandler<ImportSpeciesTask, Unit>, ImportSpeciesTaskHandler>();
  }
}
