using Krakenar.Contracts.Search;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Moves.Models;

namespace PokeCraft.Cms.Core.Moves.Queries;

internal record SearchMovesQuery(SearchMovesPayload Payload) : IQuery<SearchResults<Move>>;

internal class SearchMovesQueryHandler : IQueryHandler<SearchMovesQuery, SearchResults<Move>>
{
  private readonly IMoveQuerier _moveQuerier;

  public SearchMovesQueryHandler(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  public async Task<SearchResults<Move>> HandleAsync(SearchMovesQuery query, CancellationToken cancellationToken)
  {
    return await _moveQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
