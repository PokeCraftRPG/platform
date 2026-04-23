using Krakenar.EntityFrameworkCore.PostgreSQL;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Infrastructure;

namespace PokeCraft.Cms.PostgreSQL;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddPokeCraftCmsPostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = EnvironmentHelper.TryGetString("POSTGRESQLCONNSTR_Krakenar") ?? configuration.GetConnectionString("PostgreSQL");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException("The connection string for the database provider 'PostgreSQL' could not be found.", nameof(configuration));
    }

    return services
      .AddKrakenarEntityFrameworkCorePostgreSQL(connectionString)
      .AddDbContext<PokemonContext>(options => options.UseNpgsql(connectionString, options => options.MigrationsAssembly("PokeCraft.Cms.PostgreSQL")));
  }
}
