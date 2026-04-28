using Logitar.CQRS;
using PokeCraft.Cms.Compiler.Settings;
using PokeCraft.Cms.Compiler.Tasks;

namespace PokeCraft.Cms.Compiler;

internal class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddHostedService<CompilationWorker>();
    services.AddLogitarCQRS();
    services.AddSingleton(PokeApiSettings.Initialize(_configuration));

    services.AddTransient<ICommandHandler<CompileAbilitiesTask, Unit>, CompileAbilitiesTaskHandler>();
    services.AddTransient<ICommandHandler<CompileMovesTask, Unit>, CompileMovesTaskHandler>();
    services.AddTransient<ICommandHandler<CompileSpeciesTask, Unit>, CompileSpeciesTaskHandler>();
  }
}
