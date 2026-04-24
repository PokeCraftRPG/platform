using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record PublishAbilityCommand(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : ICommand;

internal class PublishAbilityCommandHandler : ICommandHandler<PublishAbilityCommand, Unit>
{
  private readonly ILogger<PublishAbilityCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public PublishAbilityCommandHandler(ILogger<PublishAbilityCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(PublishAbilityCommand command, CancellationToken cancellationToken)
  {
    ContentLocalePublished @event = command.Event;
    ContentLocale invariant = command.Invariant;
    ContentLocale locale = command.Locale;

    string streamId = @event.StreamId.Value;
    AbilityEntity? ability = await _pokemon.Abilities.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (ability is null)
    {
      ability = new AbilityEntity(command.Event);
      _pokemon.Abilities.Add(ability);
    }

    ability.Key = PokemonHelper.Normalize(locale.UniqueName.Value);
    ability.Name = locale.DisplayName?.Value;
    ability.Description = locale.Description?.Value;

    ability.Publish(@event);

    await _pokemon.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("The ability '{Ability}' was published.", ability);

    return Unit.Value;
  }
}
