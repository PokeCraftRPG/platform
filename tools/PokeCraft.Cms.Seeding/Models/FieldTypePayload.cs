using Krakenar.Contracts.Fields;

namespace PokeCraft.Cms.Seeding.Models;

internal record FieldTypePayload : CreateOrReplaceFieldTypePayload
{
  public Guid Id { get; set; }
}
