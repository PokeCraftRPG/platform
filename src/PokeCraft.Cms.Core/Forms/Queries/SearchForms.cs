using Krakenar.Contracts.Search;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Forms.Models;

namespace PokeCraft.Cms.Core.Forms.Queries;

internal record SearchFormsQuery(SearchFormsPayload Payload) : IQuery<SearchResults<Form>>;

internal class SearchFormsQueryHandler : IQueryHandler<SearchFormsQuery, SearchResults<Form>>
{
  private readonly IFormQuerier _formQuerier;

  public SearchFormsQueryHandler(IFormQuerier formQuerier)
  {
    _formQuerier = formQuerier;
  }

  public async Task<SearchResults<Form>> HandleAsync(SearchFormsQuery query, CancellationToken cancellationToken)
  {
    return await _formQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
