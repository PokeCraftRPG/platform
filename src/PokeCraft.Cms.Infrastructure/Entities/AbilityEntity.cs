using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.EntityFrameworkCore.Relational.Entities;

namespace PokeCraft.Cms.Infrastructure.Entities;

internal class AbilityEntity : Aggregate
{
  public int AbilityId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsPublished { get; private set; }

  public string Key { get; set; } = string.Empty;
  public string? Name { get; set; }
  public string? Description { get; set; }

  public List<FormAbilityEntity> Forms { get; private set; } = [];

  public AbilityEntity(ContentLocalePublished @event) : base(@event)
  {
    UniqueId = new ContentId(@event.StreamId).EntityId;
  }

  private AbilityEntity() : base()
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
