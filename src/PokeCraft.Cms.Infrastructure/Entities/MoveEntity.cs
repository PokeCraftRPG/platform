using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.Entities;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Moves;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class MoveEntity : Aggregate
{
  public int MoveId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsPublished { get; private set; }

  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public byte? Accuracy { get; set; }
  public byte? Power { get; set; }
  public byte PowerPoints { get; set; }

  public List<VarietyMoveEntity> Varieties { get; private set; } = [];

  public MoveEntity(ContentLocalePublished @event) : base(@event)
  {
    UniqueId = new ContentId(@event.StreamId).EntityId;
  }

  private MoveEntity() : base()
  {
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
