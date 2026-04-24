using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record PublishSpeciesCommand(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : ICommand;

internal class PublishSpeciesCommandHandler : ICommandHandler<PublishSpeciesCommand, Unit>
{
  private readonly ILogger<PublishSpeciesCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public PublishSpeciesCommandHandler(ILogger<PublishSpeciesCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(PublishSpeciesCommand command, CancellationToken cancellationToken)
  {
    ContentLocalePublished @event = command.Event;
    ContentLocale invariant = command.Invariant;
    ContentLocale locale = command.Locale;

    string streamId = @event.StreamId.Value;
    SpeciesEntity? species = await _pokemon.Species.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (species is null)
    {
      species = new SpeciesEntity(command.Event);
      _pokemon.Species.Add(species);
    }

    List<ValidationFailure> errors = new(capacity: 4);

    species.Number = (int)invariant.GetNumber(SpeciesDefinition.Number);
    SetCategory(species, invariant, errors);

    species.Key = PokemonHelper.Normalize(locale.UniqueName.Value);
    species.Name = locale.DisplayName?.Value;
    species.Description = locale.Description?.Value;

    species.BaseFriendship = (byte)invariant.GetNumber(SpeciesDefinition.BaseFriendship);
    species.CatchRate = (byte)invariant.GetNumber(SpeciesDefinition.CatchRate);
    SetGrowthRate(species, invariant, errors);
    SetPrimaryEggGroup(species, invariant, errors);
    SetSecondaryEggGroup(species, invariant, errors);

    species.EggCycles = (byte)invariant.GetNumber(SpeciesDefinition.EggCycles);

    if (errors.Count > 0)
    {
      _pokemon.ChangeTracker.Clear();
      throw new ValidationException(errors);
    }

    species.Publish(@event);

    await _pokemon.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("The species '{Species}' was published.", species);

    return Unit.Value;
  }

  private static void SetCategory(SpeciesEntity species, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(SpeciesDefinition.Category);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out PokemonCategory category) && Enum.IsDefined(category))
      {
        species.Category = category;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(SpeciesDefinition.Category), $"'{{PropertyName}}' must be parseable as a {nameof(PokemonCategory)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(SpeciesDefinition.Category), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }

  private static void SetGrowthRate(SpeciesEntity species, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(SpeciesDefinition.GrowthRate);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out GrowthRate growthRate) && Enum.IsDefined(growthRate))
      {
        species.GrowthRate = growthRate;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(SpeciesDefinition.GrowthRate), $"'{{PropertyName}}' must be parseable as a {nameof(GrowthRate)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(SpeciesDefinition.GrowthRate), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }

  private static void SetPrimaryEggGroup(SpeciesEntity species, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(SpeciesDefinition.PrimaryEggGroup);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out EggGroup eggGroup) && Enum.IsDefined(eggGroup))
      {
        species.PrimaryEggGroup = eggGroup;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(SpeciesDefinition.PrimaryEggGroup), $"'{{PropertyName}}' must be parseable as an {nameof(EggGroup)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(SpeciesDefinition.PrimaryEggGroup), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }

  private static void SetSecondaryEggGroup(SpeciesEntity species, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(SpeciesDefinition.SecondaryEggGroup);
    if (values.Count < 1)
    {
      species.SecondaryEggGroup = null;
    }
    else if (values.Count > 1)
    {
      errors.Add(new ValidationFailure(nameof(SpeciesDefinition.SecondaryEggGroup), "'{PropertyName}' must contain at most one element.", values)
      {
        ErrorCode = ErrorCodes.TooManyValues
      });
    }
    else
    {
      string value = values.Single();
      if (Enum.TryParse(value, out EggGroup eggGroup) && Enum.IsDefined(eggGroup))
      {
        species.SecondaryEggGroup = eggGroup;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(SpeciesDefinition.SecondaryEggGroup), $"'{{PropertyName}}' must be parseable as an {nameof(EggGroup)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
  }
}
