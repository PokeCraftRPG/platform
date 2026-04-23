using Microsoft.EntityFrameworkCore;

namespace PokeCraft.Cms.Infrastructure;

public class PokemonContext : DbContext
{
  public PokemonContext(DbContextOptions<PokemonContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
  }
}
