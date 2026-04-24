using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record UnpublishMoveCommand(ContentLocaleUnpublished Event) : ICommand;

internal class UnpublishMoveCommandHandler : ICommandHandler<UnpublishMoveCommand, Unit>
{
  private readonly ILogger<UnpublishMoveCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public UnpublishMoveCommandHandler(ILogger<UnpublishMoveCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(UnpublishMoveCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleUnpublished @event = command.Event;

    string streamId = @event.StreamId.Value;
    MoveEntity? move = await _pokemon.Moves.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (move is null)
    {
      _logger.LogWarning("The move 'StreamId={StreamId}' was not found.", streamId);
    }
    else
    {
      move.Unpublish(@event);

      await _pokemon.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("The move '{Move}' was unpublished.", move);
    }

    return Unit.Value;
  }
}
