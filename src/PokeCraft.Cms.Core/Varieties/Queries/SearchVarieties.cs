using Krakenar.Contracts.Search;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Varieties.Models;

namespace PokeCraft.Cms.Core.Varieties.Queries;

internal record SearchVarietiesQuery(SearchVarietiesPayload Payload) : IQuery<SearchResults<Variety>>;

internal class SearchVarietiesQueryHandler : IQueryHandler<SearchVarietiesQuery, SearchResults<Variety>>
{
  private readonly IVarietyQuerier _varietyQuerier;

  public SearchVarietiesQueryHandler(IVarietyQuerier varietyQuerier)
  {
    _varietyQuerier = varietyQuerier;
  }

  public async Task<SearchResults<Variety>> HandleAsync(SearchVarietiesQuery query, CancellationToken cancellationToken)
  {
    return await _varietyQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
