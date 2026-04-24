using Krakenar.Contracts.Search;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Abilities.Models;

namespace PokeCraft.Cms.Core.Abilities.Queries;

internal record SearchAbilitiesQuery(SearchAbilitiesPayload Payload) : IQuery<SearchResults<Ability>>;

internal class SearchAbilitiesQueryHandler : IQueryHandler<SearchAbilitiesQuery, SearchResults<Ability>>
{
  private readonly IAbilityQuerier _abilityQuerier;

  public SearchAbilitiesQueryHandler(IAbilityQuerier abilityQuerier)
  {
    _abilityQuerier = abilityQuerier;
  }

  public async Task<SearchResults<Ability>> HandleAsync(SearchAbilitiesQuery query, CancellationToken cancellationToken)
  {
    return await _abilityQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
