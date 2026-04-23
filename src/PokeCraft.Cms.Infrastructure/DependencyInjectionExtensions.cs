using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.EntityFrameworkCore.Relational.Handlers;
using Krakenar.Infrastructure;
using Krakenar.Infrastructure.Commands;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace PokeCraft.Cms.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftCmsInfrastructure(this IServiceCollection services)
  {
    return services
      .AddKrakenarInfrastructure()
      .AddKrakenarEntityFrameworkCoreRelational()
      .AddQueriers()
      .AddTransient<ICommandHandler<MigrateDatabase, Unit>, MigrateDatabaseCommandHandler>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services;
  }
}
