using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal record PublishMoveCommand(ContentLocalePublished Event, ContentLocale Invariant, ContentLocale Locale) : ICommand;

internal class PublishMoveCommandHandler : ICommandHandler<PublishMoveCommand, Unit>
{
  private readonly ILogger<PublishMoveCommandHandler> _logger;
  private readonly PokemonContext _pokemon;

  public PublishMoveCommandHandler(ILogger<PublishMoveCommandHandler> logger, PokemonContext pokemon)
  {
    _logger = logger;
    _pokemon = pokemon;
  }

  public async Task<Unit> HandleAsync(PublishMoveCommand command, CancellationToken cancellationToken)
  {
    ContentLocalePublished @event = command.Event;
    ContentLocale invariant = command.Invariant;
    ContentLocale locale = command.Locale;

    string streamId = @event.StreamId.Value;
    MoveEntity? move = await _pokemon.Moves.SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (move is null)
    {
      move = new MoveEntity(command.Event);
      _pokemon.Moves.Add(move);
    }

    List<ValidationFailure> errors = new(capacity: 2);

    SetType(move, invariant, errors);
    SetCategory(move, invariant, errors);

    move.Key = PokemonHelper.Normalize(locale.UniqueName.Value);
    move.Name = locale.DisplayName?.Value;
    move.Description = locale.Description?.Value;

    byte accuracy = (byte)invariant.GetNumber(MoveDefinition.Accuracy);
    move.Accuracy = accuracy < 1 ? null : accuracy;
    byte power = (byte)invariant.GetNumber(MoveDefinition.Power);
    move.Power = power < 1 ? null : power;
    move.PowerPoints = (byte)invariant.GetNumber(MoveDefinition.PowerPoints);

    if (errors.Count > 0)
    {
      _pokemon.ChangeTracker.Clear();
      throw new ValidationException(errors);
    }

    move.Publish(@event);

    await _pokemon.SaveChangesAsync(cancellationToken);
    _logger.LogInformation("The move '{Move}' was published.", move);

    return Unit.Value;
  }

  private static void SetCategory(MoveEntity move, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(MoveDefinition.Category);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out MoveCategory category) && Enum.IsDefined(category))
      {
        move.Category = category;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(MoveDefinition.Category), $"'{{PropertyName}}' must be parseable as a {nameof(MoveCategory)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(MoveDefinition.Category), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }

  private static void SetType(MoveEntity move, ContentLocale invariant, List<ValidationFailure> errors)
  {
    IReadOnlyCollection<string> values = invariant.GetSelect(MoveDefinition.Type);
    if (values.Count == 1)
    {
      string value = values.Single();
      if (Enum.TryParse(value, out PokemonType type) && Enum.IsDefined(type))
      {
        move.Type = type;
      }
      else
      {
        errors.Add(new ValidationFailure(nameof(MoveDefinition.Type), $"'{{PropertyName}}' must be parseable as a {nameof(PokemonType)}.", value)
        {
          ErrorCode = ErrorCodes.InvalidEnumValue
        });
      }
    }
    else
    {
      errors.Add(new ValidationFailure(nameof(MoveDefinition.Type), "'{PropertyName}' must contain exactly one element.", values)
      {
        ErrorCode = values.Count < 1 ? ErrorCodes.EmptyValue : ErrorCodes.TooManyValues
      });
    }
  }
}
