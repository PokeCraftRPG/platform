using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Moves.Models;

namespace PokeCraft.Cms.Core.Moves;

public interface IMoveQuerier
{
  Task<Move?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Move?> ReadAsync(string key, CancellationToken cancellationToken = default);
  Task<SearchResults<Move>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken = default);
}
