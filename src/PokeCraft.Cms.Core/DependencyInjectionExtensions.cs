using Krakenar.Core;
using Microsoft.Extensions.DependencyInjection;

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
    return services;
  }
}
