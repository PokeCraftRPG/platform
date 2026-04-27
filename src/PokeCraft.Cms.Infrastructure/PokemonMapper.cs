using Krakenar.Contracts;
using Krakenar.Contracts.Actors;
using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Cms.Core.Abilities;
using PokeCraft.Cms.Core.Abilities.Models;
using PokeCraft.Cms.Core.Forms.Models;
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

  public Form ToForm(FormEntity source)
  {
    if (source.Variety is null || !source.Variety.IsPublished)
    {
      throw new ArgumentException("The variety is required and must be published.", nameof(source));
    }

    return ToForm(source, ToVariety(source.Variety));
  }
  public Form ToForm(FormEntity source, Variety variety)
  {
    Form destination = new()
    {
      Id = source.UniqueId,
      Variety = variety,
      IsDefault = source.IsDefault,
      Key = source.Key,
      Name = source.Name,
      Description = source.Description,
      HasGenderDifferences = source.HasGenderDifferences,
      IsBattleOnly = source.IsBattleOnly,
      IsMega = source.IsMega,
      Height = source.Height,
      Weight = source.Weight
    };
    destination.Types.Primary = source.PrimaryType;
    destination.Types.Secondary = source.SecondaryType;

    foreach (FormAbilityEntity entity in source.Abilities)
    {
      if (entity.Ability is null)
      {
        throw new ArgumentException($"The ability 'AbilityId={entity.AbilityId}' is required.", nameof(source));
      }
      else if (entity.Ability.IsPublished)
      {
        Ability ability = ToAbility(entity.Ability);
        switch (entity.Slot)
        {
          case AbilitySlot.Primary:
            destination.Abilities.Primary = ability;
            break;
          case AbilitySlot.Secondary:
            destination.Abilities.Secondary = ability;
            break;
          case AbilitySlot.Hidden:
            destination.Abilities.Hidden = ability;
            break;
          default:
            throw new ArgumentOutOfRangeException(nameof(source), entity.Slot, "The ability slot is not supported.");
        }
      }
    }

    destination.BaseStatistics.HP = source.BaseHP;
    destination.BaseStatistics.Attack = source.BaseAttack;
    destination.BaseStatistics.Defense = source.BaseDefense;
    destination.BaseStatistics.SpecialAttack = source.BaseSpecialAttack;
    destination.BaseStatistics.SpecialDefense = source.BaseSpecialDefense;
    destination.BaseStatistics.Speed = source.BaseSpeed;

    destination.Yield.Experience = source.YieldExperience;
    destination.Yield.HP = source.YieldHP;
    destination.Yield.Attack = source.YieldAttack;
    destination.Yield.Defense = source.YieldDefense;
    destination.Yield.SpecialAttack = source.YieldSpecialAttack;
    destination.Yield.SpecialDefense = source.YieldSpecialDefense;
    destination.Yield.Speed = source.YieldSpeed;

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
      EggCycles = source.EggCycles
    };
    destination.EggGroups.Primary = source.PrimaryEggGroup;
    destination.EggGroups.Secondary = source.SecondaryEggGroup;

    foreach (VarietyEntity variety in source.Varieties)
    {
      if (variety.IsPublished)
      {
        destination.Varieties.Add(ToVariety(variety, destination));
      }
    }

    MapAggregate(source, destination);

    return destination;
  }

  public Variety ToVariety(VarietyEntity source)
  {
    if (source.Species is null || !source.Species.IsPublished)
    {
      throw new ArgumentException("The species is required and must be published.", nameof(source));
    }

    return ToVariety(source, ToSpecies(source.Species));
  }
  public Variety ToVariety(VarietyEntity source, PokemonSpecies species)
  {
    Variety destination = new()
    {
      Id = source.UniqueId,
      Species = species,
      IsDefault = source.IsDefault,
      Key = source.Key,
      Name = source.Name,
      Genus = source.Genus,
      Description = source.Description,
      CanChangeForm = source.CanChangeForm,
      GenderRatio = source.GenderRatio
    };

    foreach (FormEntity form in source.Forms)
    {
      if (form.IsPublished)
      {
        destination.Forms.Add(ToForm(form, destination));
      }
    }

    foreach (VarietyMoveEntity entity in source.Moves)
    {
      if (entity.Move is null)
      {
        throw new ArgumentException($"The move 'MoveId={entity.MoveId}' is required.", nameof(source));
      }
      else if (entity.Move.IsPublished)
      {
        destination.Moves.Add(new VarietyMove
        {
          Move = ToMove(entity.Move),
          Method = entity.Method,
          Level = entity.Level
        });
      }
    }

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
