using Krakenar.Contracts.Users;

namespace PokeCraft.Cms.Seeding.Models;

internal record UserPayload : CreateOrReplaceUserPayload
{
  public Guid? Id { get; set; }
}
