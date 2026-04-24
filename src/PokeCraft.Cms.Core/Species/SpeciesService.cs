using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Species.Models;
using PokeCraft.Cms.Core.Species.Queries;

namespace PokeCraft.Cms.Core.Species;

public interface ISpeciesService
{
  Task<PokemonSpecies?> ReadAsync(Guid? id = null, string? key = null, int? number = null, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonSpecies>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
}

internal class SpeciesService : ISpeciesService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<ISpeciesService, SpeciesService>();
    services.AddTransient<IQueryHandler<ReadSpeciesQuery, PokemonSpecies?>, ReadSpeciesQueryHandler>();
    services.AddTransient<IQueryHandler<SearchSpeciesQuery, SearchResults<PokemonSpecies>>, SearchSpeciesQueryHandler>();
  }

  private readonly IQueryBus _queryBus;

  public SpeciesService(IQueryBus queryBus)
  {
    _queryBus = queryBus;
  }

  public async Task<PokemonSpecies?> ReadAsync(Guid? id, string? key, int? number, CancellationToken cancellationToken)
  {
    ReadSpeciesQuery query = new(id, key, number);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public Task<SearchResults<PokemonSpecies>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken)
  {
    SearchSpeciesQuery query = new(payload);
    return _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
