using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Abilities.Models;

namespace PokeCraft.Cms.Core.Abilities;

public interface IAbilityQuerier
{
  Task<Ability?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Ability?> ReadAsync(string key, CancellationToken cancellationToken = default);
  Task<SearchResults<Ability>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken = default);
}
