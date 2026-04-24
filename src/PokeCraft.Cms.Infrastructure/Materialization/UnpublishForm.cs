using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record UnpublishFormCommand(ContentLocaleUnpublished Event) : ICommand;

internal class UnpublishFormCommandHandler : ICommandHandler<UnpublishFormCommand, Unit>
{
  private readonly ILogger<UnpublishFormCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public UnpublishFormCommandHandler(ILogger<UnpublishFormCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(UnpublishFormCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleUnpublished @event = command.Event;

    string streamId = @event.StreamId.Value;
    FormEntity? form = await _pokemon.Forms.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (form is null)
    {
      _logger.LogWarning("The form 'StreamId={StreamId}' was not found.", streamId);
    }
    else
    {
      form.Unpublish(@event);

      await _pokemon.SaveChangesAsync(cancellationToken);
      _logger.LogInformation("The form '{Form}' was unpublished.", form);
    }

    return Unit.Value;
  }
}
