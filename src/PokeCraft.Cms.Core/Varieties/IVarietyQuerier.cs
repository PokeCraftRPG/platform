using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Varieties.Models;

namespace PokeCraft.Cms.Core.Varieties;

public interface IVarietyQuerier
{
  Task<Variety?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Variety?> ReadAsync(string key, CancellationToken cancellationToken = default);
  Task<SearchResults<Variety>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken = default);
}
