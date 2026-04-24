using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record UnpublishAbilityCommand(ContentLocaleUnpublished Event) : ICommand;

internal class UnpublishAbilityCommandHandler : ICommandHandler<UnpublishAbilityCommand, Unit>
{
  private readonly ILogger<UnpublishAbilityCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public UnpublishAbilityCommandHandler(ILogger<UnpublishAbilityCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(UnpublishAbilityCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleUnpublished @event = command.Event;

    string streamId = @event.StreamId.Value;
    AbilityEntity? ability = await _pokemon.Abilities.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (ability is null)
    {
      _logger.LogWarning("The ability 'StreamId={StreamId}' was not found.", streamId);
    }
    else
    {
      ability.Unpublish(@event);

      await _pokemon.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("The ability '{Ability}' was unpublished.", ability);
    }

    return Unit.Value;
  }
}
