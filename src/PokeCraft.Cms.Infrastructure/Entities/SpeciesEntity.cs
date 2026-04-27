using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Cms.Core.Species;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class SpeciesEntity : Aggregate
{
  public int SpeciesId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsPublished { get; private set; }

  public int Number { get; set; }
  public SpeciesCategory Category { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public byte BaseFriendship { get; set; }
  public byte CatchRate { get; set; }
  public GrowthRate GrowthRate { get; set; }

  public byte EggCycles { get; set; }
  public EggGroup PrimaryEggGroup { get; set; }
  public EggGroup? SecondaryEggGroup { get; set; }

  public List<VarietyEntity> Varieties { get; private set; } = [];

  public SpeciesEntity(ContentLocalePublished @event) : base(@event)
  {
    UniqueId = new ContentId(@event.StreamId).EntityId;
  }

  private SpeciesEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(includeVarieties: true);
  public IReadOnlyCollection<ActorId> GetActorIds(bool includeVarieties)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (includeVarieties)
    {
      foreach (VarietyEntity variety in Varieties)
      {
        actorIds.AddRange(variety.GetActorIds(includeSpecies: false, includeForms: true));
      }
    }
    return actorIds;
  }

  public void Publish(ContentLocalePublished @event)
  {
    Update(@event);

    IsPublished = true;
  }

  public void Unpublish(ContentLocaleUnpublished @event)
  {
    Update(@event);

    IsPublished = false;
  }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
