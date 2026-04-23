using Krakenar.Contracts.Fields;

namespace PokeCraft.Cms.Seeding.Models;

internal record FieldDefinitionPayload : CreateOrReplaceFieldDefinitionPayload
{
  public Guid Id { get; set; }
}
