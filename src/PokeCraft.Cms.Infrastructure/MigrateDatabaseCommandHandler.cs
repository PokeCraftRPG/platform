using Krakenar.EntityFrameworkCore.Relational;
using Krakenar.Infrastructure.Commands;
using Logitar.CQRS;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;

namespace PokeCraft.Cms.Infrastructure;

internal class MigrateDatabaseCommandHandler : Krakenar.EntityFrameworkCore.Relational.Handlers.MigrateDatabaseCommandHandler, ICommandHandler<MigrateDatabase, Unit>
{
  private readonly PokemonContext _pokemon;

  public MigrateDatabaseCommandHandler(EventContext eventSourcing, KrakenarContext krakenar, PokemonContext pokemon)
    : base(eventSourcing, krakenar)
  {
    _pokemon = pokemon;
  }

  public override async Task<Unit> HandleAsync(MigrateDatabase command, CancellationToken cancellationToken)
  {
    await base.HandleAsync(command, cancellationToken);

    await _pokemon.Database.MigrateAsync(cancellationToken);

    return Unit.Value;
  }
}
