using FluentValidation;
using FluentValidation.Results;
using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Logitar;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PokeCraft.Cms.Core.Moves;
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
    VarietyEntity? variety = await _pokemon.Varieties
      .Include(x => x.Moves).ThenInclude(x => x.Move)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);
    if (variety is null)
    {
      variety = new VarietyEntity(command.Event);
      _pokemon.Varieties.Add(variety);
    }

    List<ValidationFailure> errors = [];

    await SetSpeciesAsync(variety, invariant, errors, cancellationToken);
    variety.IsDefault = invariant.GetBoolean(VarietyDefinition.IsDefault);

    variety.Key = PokemonHelper.Normalize(locale.UniqueName.Value);
    variety.Name = locale.DisplayName?.Value;
    variety.Genus = locale.TryGetString(VarietyDefinition.Genus);
    variety.Description = locale.Description?.Value;

    variety.CanChangeForm = invariant.GetBoolean(VarietyDefinition.CanChangeForm);
    variety.GenderRatio = (byte?)invariant.TryGetNumber(VarietyDefinition.GenderRatio);

    await SetMovesAsync(variety, invariant, errors, cancellationToken);

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

  private async Task SetMovesAsync(VarietyEntity variety, ContentLocale invariant, List<ValidationFailure> errors, CancellationToken cancellationToken)
  {
    string[] lines = invariant.GetString(VarietyDefinition.Moves).Remove("\r").Split('\n');
    Dictionary<string, MoveLearning> varietyMoves = new(capacity: lines.Length);
    for (int i = 0; i < lines.Length; i++)
    {
      string line = lines[i];
      if (!string.IsNullOrWhiteSpace(line))
      {
        bool isValid = true;
        string[] values = line.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (values.Length == 2)
        {
          string learning = values.First().ToLowerInvariant();
          string move = PokemonHelper.Normalize(values.Last());
          switch (learning)
          {
            case "evolution":
              varietyMoves[move] = new MoveLearning(LearningMethod.Evolution, Level: null);
              break;
            case "reminder":
              varietyMoves[move] = new MoveLearning(LearningMethod.Reminder, Level: null);
              break;
            default:
              if (int.TryParse(learning, out int level) && level > 0 && level <= 100)
              {
                varietyMoves[move] = new MoveLearning(LearningMethod.Level, level);
              }
              else
              {
                isValid = false;
              }
              break;
          }
        }
        else
        {
          isValid = false;
        }

        if (!isValid)
        {
          errors.Add(new ValidationFailure($"{nameof(VarietyDefinition.Moves)}[{i}]", "'{PropertyName}' must match the format '{Evolution|Reminder|{Level:1-100}}:{MoveKey}'.", line)
          {
            ErrorCode = ErrorCodes.InvalidFormat
          });
        }
      }
    }

    foreach (VarietyMoveEntity varietyMove in variety.Moves)
    {
      if (varietyMove.Move is not null)
      {
        if (varietyMoves.TryGetValue(varietyMove.Move.Key, out MoveLearning? learning))
        {
          varietyMove.Method = learning.Method;
          varietyMove.Level = learning.Level;
          varietyMoves.Remove(varietyMove.Move.Key);
        }
        else
        {
          _pokemon.VarietyMoves.Remove(varietyMove);
        }
      }
    }

    if (varietyMoves.Count > 0)
    {
      Dictionary<string, MoveEntity> moves = await _pokemon.Moves
        .Where(x => varietyMoves.Keys.Contains(x.Key))
        .ToDictionaryAsync(x => x.Key, x => x, cancellationToken);
      foreach (KeyValuePair<string, MoveLearning> varietyMove in varietyMoves)
      {
        if (moves.TryGetValue(varietyMove.Key, out MoveEntity? move))
        {
          variety.Moves.Add(new VarietyMoveEntity(variety, move, varietyMove.Value.Method, varietyMove.Value.Level));
        }
        else
        {
          errors.Add(new ValidationFailure(nameof(VarietyDefinition.Moves), "'{PropertyName}' must reference an existing entity.", varietyMove.Key)
          {
            ErrorCode = ErrorCodes.EntityNotFound
          });
        }
      }
    }
  }
  private record MoveLearning(LearningMethod Method, int? Level);

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
