using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record UnpublishSpeciesCommand(ContentLocaleUnpublished Event) : ICommand;

internal class UnpublishSpeciesCommandHandler : ICommandHandler<UnpublishSpeciesCommand, Unit>
{
  private readonly ILogger<UnpublishSpeciesCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public UnpublishSpeciesCommandHandler(ILogger<UnpublishSpeciesCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(UnpublishSpeciesCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleUnpublished @event = command.Event;

    string streamId = @event.StreamId.Value;
    SpeciesEntity? species = await _pokemon.Species.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (species is null)
    {
      _logger.LogWarning("The species 'StreamId={StreamId}' was not found.", streamId);
    }
    else
    {
      species.Unpublish(@event);

      await _pokemon.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("The species '{Species}' was unpublished.", species);
    }

    return Unit.Value;
  }
}
