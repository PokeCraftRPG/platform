using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record PublishFormCommand(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : ICommand;

internal class PublishFormCommandHandler : ICommandHandler<PublishFormCommand, Unit>
{
  private readonly ILogger<PublishFormCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public PublishFormCommandHandler(ILogger<PublishFormCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(PublishFormCommand command, CancellationToken cancellationToken)
  {
    ContentLocalePublished @event = command.Event;
    ContentLocale invariant = command.Invariant;
    ContentLocale locale = command.Locale;

    string streamId = @event.StreamId.Value;
    FormEntity? form = await _pokemon.Forms.Include(x => x.Abilities).ThenInclude(x => x.Ability).SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (form is null)
    {
      form = new FormEntity(command.Event);
      _pokemon.Forms.Add(form);
    }

    List<ValidationFailure> errors = new(capacity: 3);

    await SetVarietyAsync(form, invariant, errors, cancellationToken);
    form.IsDefault = invariant.GetBoolean(FormDefinition.IsDefault);

    form.Key = locale.UniqueName.Value.ToLowerInvariant();
    form.Name = locale.DisplayName?.Value;
    form.Description = locale.Description?.Value;

    form.HasGenderDifferences = invariant.GetBoolean(FormDefinition.HasGenderDifferences);
    form.IsBattleOnly = invariant.GetBoolean(FormDefinition.IsBattleOnly);
    form.IsMega = invariant.GetBoolean(FormDefinition.IsMega);

    form.Height = (int)invariant.GetNumber(FormDefinition.Height);
    form.Weight = (int)invariant.GetNumber(FormDefinition.Weight);

    SetPrimaryType(form, invariant, errors);
    SetSecondaryType(form, invariant, errors);

    // TODO(fpion): Abilities

    form.BaseHP = (byte)invariant.GetNumber(FormDefinition.BaseHP);
    form.BaseAttack = (byte)invariant.GetNumber(FormDefinition.BaseAttack);
    form.BaseDefense = (byte)invariant.GetNumber(FormDefinition.BaseDefense);
    form.BaseSpecialAttack = (byte)invariant.GetNumber(FormDefinition.BaseSpecialAttack);
    form.BaseSpecialDefense = (byte)invariant.GetNumber(FormDefinition.BaseSpecialDefense);
    form.BaseSpeed = (byte)invariant.GetNumber(FormDefinition.BaseSpeed);

    form.YieldExperience = (int)invariant.GetNumber(FormDefinition.YieldExperience);
    form.YieldHP = (byte)invariant.GetNumber(FormDefinition.YieldHP);
    form.YieldAttack = (byte)invariant.GetNumber(FormDefinition.YieldAttack);
    form.YieldDefense = (byte)invariant.GetNumber(FormDefinition.YieldDefense);
    form.YieldSpecialAttack = (byte)invariant.GetNumber(FormDefinition.YieldSpecialAttack);
    form.YieldSpecialDefense = (byte)invariant.GetNumber(FormDefinition.YieldSpecialDefense);
    form.YieldSpeed = (byte)invariant.GetNumber(FormDefinition.YieldSpeed);

    if (errors.Count > 0)
    {
      _pokemon.ChangeTracker.Clear();
      throw new ValidationException(errors);
    }

    form.Publish(@event);

    await _pokemon.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("The form '{Form}' was published.", form);

    return Unit.Value;
  }

  private static void SetPrimaryType(FormEntity form, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(FormDefinition.PrimaryType);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out PokemonType type) && Enum.IsDefined(type))
      {
        form.PrimaryType = type;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(FormDefinition.PrimaryType), $"'{{PropertyName}}' must be parseable as a {nameof(PokemonType)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(FormDefinition.PrimaryType), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }

  private static void SetSecondaryType(FormEntity form, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(FormDefinition.SecondaryType);
    if (values.Count < 1)
    {
      form.SecondaryType = null;
    }
    else if (values.Count > 1)
    {
      errors.Add(new ValidationFailure(nameof(FormDefinition.SecondaryType), "'{PropertyName}' must contain at most one element.", values)
      {
        ErrorCode = ErrorCodes.TooManyValues
      });
    }
    else
    {
      string value = values.Single();
      if (Enum.TryParse(value, out PokemonType type) && Enum.IsDefined(type))
      {
        form.SecondaryType = type;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(FormDefinition.SecondaryType), $"'{{PropertyName}}' must be parseable as a {nameof(PokemonType)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
  }

  private async Task SetVarietyAsync(FormEntity form, ContentLocale invariant, List<ValidationFailure> errors, CancellationToken cancellationToken)
  {
    IReadOnlyCollection<Guid> varietyIds = invariant.GetRelatedContent(FormDefinition.Variety);
    if (varietyIds.Count == 1)
    {
      Guid varietyId = varietyIds.Single();
      VarietyEntity? variety = await _pokemon.Varieties.SingleOrDefaultAsync(x => x.UniqueId == varietyId, cancellationToken);
      if (variety is null)
      {
        errors.Add(new ValidationFailure(nameof(FormDefinition.Variety), "'{PropertyName}' must reference an existing entity.", varietyId)
        {
          ErrorCode = ErrorCodes.EntityNotFound
        });
      }
      else
      {
        form.SetVariety(variety);
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(FormDefinition.Variety), "'{PropertyName}' must contain exactly one element.", varietyIds)
      {
        ErrorCode = varietyIds.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }
}
