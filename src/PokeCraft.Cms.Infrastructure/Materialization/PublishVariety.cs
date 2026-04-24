using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record PublishVarietyCommand(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : ICommand;

internal class PublishVarietyCommandHandler : ICommandHandler<PublishVarietyCommand, Unit>
{
  private readonly ILogger<PublishVarietyCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public PublishVarietyCommandHandler(ILogger<PublishVarietyCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(PublishVarietyCommand command, CancellationToken cancellationToken)
  {
    ContentLocalePublished @event = command.Event;
    ContentLocale invariant = command.Invariant;
    ContentLocale locale = command.Locale;

    string streamId = @event.StreamId.Value;
    VarietyEntity? variety = await _pokemon.Varieties.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (variety is null)
    {
      variety = new VarietyEntity(command.Event);
      _pokemon.Varieties.Add(variety);
    }

    List<ValidationFailure> errors = new(capacity: 1);

    await SetSpeciesAsync(variety, invariant, errors, cancellationToken);
    variety.IsDefault = invariant.GetBoolean(VarietyDefinition.IsDefault);

    variety.Key = locale.UniqueName.Value.ToLowerInvariant();
    variety.Name = locale.DisplayName?.Value;
    variety.Genus = locale.TryGetString(VarietyDefinition.Genus);
    variety.Description = locale.Description?.Value;

    variety.CanChangeForm = invariant.GetBoolean(VarietyDefinition.CanChangeForm);
    variety.GenderRatio = (byte?)invariant.TryGetNumber(VarietyDefinition.GenderRatio);

    if (errors.Count > 0)
    {
      _pokemon.ChangeTracker.Clear();
      throw new ValidationException(errors);
    }

    variety.Publish(@event);

    await _pokemon.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("The variety '{Variety}' was published.", variety);

    return Unit.Value;
  }

  private async Task SetSpeciesAsync(VarietyEntity variety, ContentLocale invariant, List<ValidationFailure> errors, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Guid> speciesIds = invariant.GetRelatedContent(VarietyDefinition.Species);
    if (speciesIds.Count == 1)
    {
      Guid speciesId = speciesIds.Single();
      SpeciesEntity? species = await _pokemon.Species.SingleOrDefaultAsync(x => x.UniqueId == speciesId, cancellationToken);
      if (species is null)
      {
        errors.Add(new ValidationFailure(nameof(VarietyDefinition.Species), "'{PropertyName}' must reference an existing entity.", speciesId)
        {
          ErrorCode = ErrorCodes.EntityNotFound
        });
      }
      else
      {
        variety.SetSpecies(species);
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(VarietyDefinition.Species), "'{PropertyName}' must contain exactly one element.", speciesIds)
      {
        ErrorCode = speciesIds.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }
}
