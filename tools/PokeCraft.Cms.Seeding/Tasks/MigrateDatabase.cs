using Krakenar.Infrastructure.Commands;
using Logitar.CQRS;

namespace PokeCraft.Cms.Seeding.Tasks;

internal class MigrateDatabaseTask : SeedingTask
{
  public override string? Description => "Migrates Krakenar database.";
}

internal class MigrateDatabaseTaskHandler : ICommandHandler<MigrateDatabaseTask, Unit>
{
  private readonly ICommandHandler<MigrateDatabase, Unit> _handler;

  public MigrateDatabaseTaskHandler(ICommandHandler<MigrateDatabase, Unit> handler)
  {
    _handler = handler;
  }

  public async Task<Unit> HandleAsync(MigrateDatabaseTask _, CancellationToken cancellationToken)
  {
    MigrateDatabase command = new();
    await _handler.HandleAsync(command, cancellationToken);

    return Unit.Value;
  }
}
