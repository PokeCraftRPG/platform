using Krakenar.MongoDB;
using Krakenar.Web;
using Krakenar.Web.Middlewares;
using Krakenar.Web.Settings;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Extensions;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.PostgreSQL;

namespace PokeCraft.Cms;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddPokeCraftCmsCore();
    services.AddPokeCraftCmsInfrastructure();
    services.AddPokeCraftCmsPostgreSQL(_configuration);

    services.AddKrakenarWeb(_configuration);
    services.AddKrakenarMongoDB(_configuration);

    AdminSettings? adminSettings = services.Where(x => x.ServiceType.Equals(typeof(AdminSettings)) && x.ImplementationInstance is AdminSettings)
      .FirstOrDefault()?.ImplementationInstance as AdminSettings
      ?? throw new InvalidOperationException($"The {nameof(AdminSettings)} service has not been registered.");
    if (adminSettings.EnableSwagger)
    {
      services.AddKrakenarSwagger(adminSettings);
    }

    services.AddApplicationInsightsTelemetry();
    services.AddHealthChecks().AddDbContextCheck<PokemonContext>();
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (builder is WebApplication application)
    {
      Configure(application);
    }
  }
  public virtual void Configure(WebApplication application)
  {
    AdminSettings adminSettings = application.Services.GetRequiredService<AdminSettings>();
    if (adminSettings.EnableSwagger)
    {
      application.UseKrakenarSwagger(adminSettings);
    }

    application.UseHttpsRedirection();
    application.UseCors();
    application.UseStaticFiles();
    application.UseExceptionHandler();
    application.UseSession();
    application.UseMiddleware<Logging>();
    application.UseMiddleware<RenewSession>();
    application.UseMiddleware<RedirectNotFound>();
    application.UseAuthentication();
    application.UseAuthorization();
    application.UseMiddleware<ResolveRealm>();
    application.UseMiddleware<ResolveUser>();

    application.MapControllers();
    application.MapControllerRoute(name: "Admin", pattern: $"{adminSettings.BasePath}/{{**anything}}", defaults: new { Controller = "Admin", Action = "Index" });
    application.MapHealthChecks("/health");
  }
}
