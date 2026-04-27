using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using Logitar;
using Logitar.EventSourcing;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Forms;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class FormEntity : Aggregate
{
  public int FormId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsPublished { get; private set; }

  public VarietyEntity? Variety { get; private set; }
  public int VarietyId { get; private set; }
  public FormKind Kind { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public bool HasGenderDifferences { get; set; }

  public int Height { get; set; }
  public int Weight { get; set; }

  public PokemonType PrimaryType { get; set; }
  public PokemonType? SecondaryType { get; set; }

  public byte BaseHP { get; set; }
  public byte BaseAttack { get; set; }
  public byte BaseDefense { get; set; }
  public byte BaseSpecialAttack { get; set; }
  public byte BaseSpecialDefense { get; set; }
  public byte BaseSpeed { get; set; }

  public int YieldExperience { get; set; }
  public byte YieldHP { get; set; }
  public byte YieldAttack { get; set; }
  public byte YieldDefense { get; set; }
  public byte YieldSpecialAttack { get; set; }
  public byte YieldSpecialDefense { get; set; }
  public byte YieldSpeed { get; set; }

  public List<FormAbilityEntity> Abilities { get; private set; } = [];

  public FormEntity(ContentLocalePublished @event) : base(@event)
  {
    UniqueId = new ContentId(@event.StreamId).EntityId;
  }

  private FormEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(includeVariety: true);
  public IReadOnlyCollection<ActorId> GetActorIds(bool includeVariety)
  {
    HashSet<ActorId> actorIds = new(base.GetActorIds());
    if (includeVariety && Variety is not null)
    {
      actorIds.AddRange(Variety.GetActorIds(includeSpecies: true, includeForms: false));
    }
    foreach (FormAbilityEntity entity in Abilities)
    {
      if (entity.Ability is not null)
      {
        actorIds.AddRange(entity.Ability.GetActorIds());
      }
    }
    return actorIds;
  }

  public void Publish(ContentLocalePublished @event)
  {
    Update(@event);

    IsPublished = true;
  }

  public void SetVariety(VarietyEntity variety)
  {
    Variety = variety;
    VarietyId = variety.VarietyId;
  }

  public void Unpublish(ContentLocaleUnpublished @event)
  {
    Update(@event);

    IsPublished = false;
  }

  public override string ToString() => $"{Name ?? Key} | {base.ToString()}";
}
