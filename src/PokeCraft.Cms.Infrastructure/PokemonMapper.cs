using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Cms.Core.Abilities.Models;
using PokeCraft.Cms.Core.Moves.Models;
using PokeCraft.Cms.Core.Species.Models;
using PokeCraft.Cms.Core.Varieties.Models;
using PokeCraft.Cms.Infrastructure.Entities;
using AggregateEntity = Krakenar.EntityFrameworkCore.Relational.Entities.Aggregate;

namespace PokeCraft.Cms.Infrastructure;

internal class PokemonMapper
{
  private readonly Dictionary<ActorId, Actor> _actors = [];
  private readonly Actor _system = new();

  public PokemonMapper()
  {
  }

  public PokemonMapper(IEnumerable<KeyValuePair<ActorId, Actor>> actors)
  {
    foreach (KeyValuePair<ActorId, Actor> actor in actors)
    {
      _actors[actor.Key] = actor.Value;
    }
  }

  public Ability ToAbility(AbilityEntity source)
  {
    Ability destination = new()
    {
      Id = source.UniqueId,
      Key = source.Key,
      Name = source.Name,
      Description = source.Description
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Move ToMove(MoveEntity source)
  {
    Move destination = new()
    {
      Id = source.UniqueId,
      Type = source.Type,
      Category = source.Category,
      Key = source.Key,
      Name = source.Name,
      Description = source.Description,
      Accuracy = source.Accuracy,
      Power = source.Power,
      PowerPoints = source.PowerPoints
    };

    MapAggregate(source, destination);

    return destination;
  }

  public PokemonSpecies ToSpecies(SpeciesEntity source)
  {
    PokemonSpecies destination = new()
    {
      Id = source.UniqueId,
      Number = source.Number,
      Category = source.Category,
      Key = source.Key,
      Name = source.Name,
      Description = source.Description,
      BaseFriendship = source.BaseFriendship,
      CatchRate = source.CatchRate,
      GrowthRate = source.GrowthRate,
      EggCycles = source.EggCycles,
      PrimaryEggGroup = source.PrimaryEggGroup,
      SecondaryEggGroup = source.SecondaryEggGroup
    };

    MapAggregate(source, destination);

    return destination;
  }

  public Variety ToVariety(VarietyEntity source)
  {
    if (source.Species is null)
    {
      throw new ArgumentException("The species is required.", nameof(source));
    }

    Variety destination = new()
    {
      Id = source.UniqueId,
      Species = ToSpecies(source.Species),
      IsDefault = source.IsDefault,
      Key = source.Key,
      Name = source.Name,
      Genus = source.Genus,
      Description = source.Description,
      CanChangeForm = source.CanChangeForm,
      GenderRatio = source.GenderRatio
    };

    MapAggregate(source, destination);

    return destination;
  }

  private void MapAggregate(AggregateEntity source, Aggregate destination)
  {
    destination.Version = source.Version;

    destination.CreatedBy = FindActor(source.CreatedBy);
    destination.CreatedOn = source.CreatedOn.AsUniversalTime();

    destination.UpdatedBy = FindActor(source.UpdatedBy);
    destination.UpdatedOn = source.UpdatedOn.AsUniversalTime();
  }

  private Actor FindActor(string? actorId) => TryGetActor(actorId) ?? _system;
  private Actor FindActor(ActorId? actorId) => TryGetActor(actorId) ?? _system;
  private Actor? TryGetActor(string? actorId) => TryGetActor(actorId is null ? null : new ActorId(actorId));
  private Actor? TryGetActor(ActorId? actorId) => actorId.HasValue && _actors.TryGetValue(actorId.Value, out Actor? actor) ? actor : null;
}
