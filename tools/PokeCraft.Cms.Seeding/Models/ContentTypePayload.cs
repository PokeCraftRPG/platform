using Krakenar.Contracts.Contents;

namespace PokeCraft.Cms.Seeding.Models;

internal record ContentTypePayload : CreateOrReplaceContentTypePayload
{
  public Guid Id { get; set; }

  public List<FieldDefinitionPayload> Fields { get; set; } = [];
}
