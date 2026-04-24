using Krakenar.Contracts.Search;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Core.Species.Queries;

internal record SearchSpeciesQuery(SearchSpeciesPayload Payload) : IQuery<SearchResults<PokemonSpecies>>;

internal class SearchSpeciesQueryHandler : IQueryHandler<SearchSpeciesQuery, SearchResults<PokemonSpecies>>
{
  private readonly ISpeciesQuerier _speciesQuerier;

  public SearchSpeciesQueryHandler(ISpeciesQuerier speciesQuerier)
  {
    _speciesQuerier = speciesQuerier;
  }

  public async Task<SearchResults<PokemonSpecies>> HandleAsync(SearchSpeciesQuery query, CancellationToken cancellationToken)
  {
    return await _speciesQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
