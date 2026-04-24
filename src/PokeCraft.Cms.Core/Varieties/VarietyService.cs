using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Varieties.Models;
using PokeCraft.Cms.Core.Varieties.Queries;

namespace PokeCraft.Cms.Core.Varieties;

public interface IVarietyService
{
  Task<Variety?> ReadAsync(Guid? id = null, string? key = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Variety>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken = default);
}

internal class VarietyService : IVarietyService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IVarietyService, VarietyService>();
    services.AddTransient<IQueryHandler<ReadVarietyQuery, Variety?>, ReadVarietyQueryHandler>();
    services.AddTransient<IQueryHandler<SearchVarietiesQuery, SearchResults<Variety>>, SearchVarietiesQueryHandler>();
  }

  private readonly IQueryBus _queryBus;

  public VarietyService(IQueryBus queryBus)
  {
    _queryBus = queryBus;
  }

  public async Task<Variety?> ReadAsync(Guid? id, string? key, CancellationToken cancellationToken)
  {
    ReadVarietyQuery query = new(id, key);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public Task<SearchResults<Variety>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken)
  {
    SearchVarietiesQuery query = new(payload);
    return _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
