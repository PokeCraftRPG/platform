using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Logitar.EventSourcing;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class VarietyEntity : Aggregate
{
  public int VarietyId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsPublished { get; private set; }

  public SpeciesEntity? Species { get; private set; }
  public int SpeciesId { get; private set; }
  public bool IsDefault { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Genus { get; set; }
  public string? Description { get; set; }

  public bool CanChangeForm { get; set; }
  public byte? GenderRatio { get; set; }

  public List<FormEntity> Forms { get; private set; } = [];
  public List<VarietyMoveEntity> Moves { get; private set; } = [];

  public VarietyEntity(ContentLocalePublished @event) : base(@event)
  {
    UniqueId = new ContentId(@event.StreamId).EntityId;
  }

  private VarietyEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(includeSpecies: true, includeForms: false);
  public IReadOnlyCollection<ActorId> GetActorIds(bool includeSpecies, bool includeForms)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (includeSpecies && Species is not null)
    {
      actorIds.AddRange(Species.GetActorIds(includeVarieties: false));
    }
    if (includeForms)
    {
      foreach (FormEntity form in Forms)
      {
        actorIds.AddRange(form.GetActorIds(includeVariety: false));
      }
    }
    foreach (VarietyMoveEntity entity in Moves)
    {
      if (entity.Move is not null)
      {
        actorIds.AddRange(entity.Move.GetActorIds());
      }
    }
    return actorIds;
  }

  public void Publish(ContentLocalePublished @event)
  {
    Update(@event);

    IsPublished = true;
  }

  public void SetSpecies(SpeciesEntity species)
  {
    Species = species;
    SpeciesId = species.SpeciesId;
  }

  public void Unpublish(ContentLocaleUnpublished @event)
  {
    Update(@event);

    IsPublished = false;
  }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
