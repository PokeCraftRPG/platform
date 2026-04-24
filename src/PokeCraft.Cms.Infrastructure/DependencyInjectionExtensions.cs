using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure;
using Krakenar.Infrastructure.Commands;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Abilities;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Core.Varieties;
using PokeCraft.Cms.Infrastructure.Materialization;
using PokeCraft.Cms.Infrastructure.Queriers;

namespace PokeCraft.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftCmsInfrastructure(this IServiceCollection services)
  {
    ContentLocalePublishedHandler.Register(services);
    ContentLocaleUnpublishedHandler.Register(services);
    return services
      .AddKrakenarInfrastructure()
      .AddKrakenarEntityFrameworkCoreRelational()
      .AddQueriers()
      .AddTransient<ICommandHandler<MigrateDatabase, Unit>, MigrateDatabaseCommandHandler>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services
      .AddTransient<IAbilityQuerier, AbilityQuerier>()
      .AddTransient<IMoveQuerier, MoveQuerier>()
      .AddTransient<ISpeciesQuerier, SpeciesQuerier>()
      .AddTransient<IVarietyQuerier, VarietyQuerier>();
  }
}
