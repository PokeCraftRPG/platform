using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Abilities.Models;
using PokeCraft.Cms.Core.Abilities.Queries;

namespace PokeCraft.Cms.Core.Abilities;

public interface IAbilityService
{
  Task<Ability?> ReadAsync(Guid? id = null, string? key = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Ability>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken = default);
}

internal class AbilityService : IAbilityService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IAbilityService, AbilityService>();
    services.AddTransient<IQueryHandler<ReadAbilityQuery, Ability?>, ReadAbilityQueryHandler>();
    services.AddTransient<IQueryHandler<SearchAbilitiesQuery, SearchResults<Ability>>, SearchAbilitiesQueryHandler>();
  }

  private readonly IQueryBus _queryBus;

  public AbilityService(IQueryBus queryBus)
  {
    _queryBus = queryBus;
  }

  public async Task<Ability?> ReadAsync(Guid? id, string? key, CancellationToken cancellationToken)
  {
    ReadAbilityQuery query = new(id, key);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public Task<SearchResults<Ability>> SearchAsync(SearchAbilitiesPayload payload, CancellationToken cancellationToken)
  {
    SearchAbilitiesQuery query = new(payload);
    return _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
