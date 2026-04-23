using Krakenar.Core;
using Logitar.CQRS;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.PostgreSQL;
using PokeCraft.Cms.Seeding.Settings;
using PokeCraft.Cms.Seeding.Tasks;

namespace PokeCraft.Cms.Seeding;

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

    services.AddHostedService<SeedingWorker>();
    services.AddSingleton(serviceProvider => DefaultSettings.Initialize(serviceProvider.GetRequiredService<IConfiguration>()));
    services.AddSingleton<IApplicationContext, SeedingApplicationContext>();

    services.AddTransient<ICommandHandler<InitializeConfigurationTask, Unit>, InitializeConfigurationTaskHandler>();
    services.AddTransient<ICommandHandler<MigrateDatabaseTask, Unit>, MigrateDatabaseTaskHandler>();
    services.AddTransient<ICommandHandler<SeedContentTypesTask, Unit>, SeedContentTypesTaskHandler>();
    services.AddTransient<ICommandHandler<SeedFieldTypesTask, Unit>, SeedFieldTypesTaskHandler>();
    services.AddTransient<ICommandHandler<SeedUsersTask, Unit>, SeedUsersTaskHandler>();
  }
}
