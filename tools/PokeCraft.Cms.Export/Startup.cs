using Krakenar.Core;
using Logitar.CQRS;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Export.Tasks;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.PostgreSQL;

namespace PokeCraft.Cms.Export;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddPokeCraftCmsCore();
    services.AddPokeCraftCmsInfrastructure();
    services.AddPokeCraftCmsPostgreSQL(_configuration);

    services.AddHostedService<ExportWorker>();
    services.AddSingleton<IApplicationContext, ExportApplicationContext>();

    services.AddTransient<ICommandHandler<ExportContentsTask, Unit>, ExportContentsTaskHandler>();
  }
}
