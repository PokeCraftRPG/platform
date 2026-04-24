using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record UnpublishVarietyCommand(ContentLocaleUnpublished Event) : ICommand;

internal class UnpublishVarietyCommandHandler : ICommandHandler<UnpublishVarietyCommand, Unit>
{
  private readonly ILogger<UnpublishVarietyCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public UnpublishVarietyCommandHandler(ILogger<UnpublishVarietyCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(UnpublishVarietyCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleUnpublished @event = command.Event;

    string streamId = @event.StreamId.Value;
    VarietyEntity? variety = await _pokemon.Varieties.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (variety is null)
    {
      _logger.LogWarning("The variety 'StreamId={StreamId}' was not found.", streamId);
    }
    else
    {
      variety.Unpublish(@event);

      await _pokemon.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("The variety '{Variety}' was unpublished.", variety);
    }

    return Unit.Value;
  }
}
