using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Abilities;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Species;

namespace PokeCraft.Cms.Core;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftCmsCore(this IServiceCollection services)
  {
    return services
      .AddKrakenarCore()
      .AddCoreServices();
  }

  private static IServiceCollection AddCoreServices(this IServiceCollection services)
  {
    AbilityService.Register(services);
    MoveService.Register(services);
    SpeciesService.Register(services);
    return services;
  }
}
