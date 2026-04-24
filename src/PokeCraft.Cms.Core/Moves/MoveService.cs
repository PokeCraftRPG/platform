using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Moves.Models;
using PokeCraft.Cms.Core.Moves.Queries;

namespace PokeCraft.Cms.Core.Moves;

public interface IMoveService
{
  Task<Move?> ReadAsync(Guid? id = null, string? key = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Move>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken = default);
}

internal class MoveService : IMoveService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IMoveService, MoveService>();
    services.AddTransient<IQueryHandler<ReadMoveQuery, Move?>, ReadMoveQueryHandler>();
    services.AddTransient<IQueryHandler<SearchMovesQuery, SearchResults<Move>>, SearchMovesQueryHandler>();
  }

  private readonly IQueryBus _queryBus;

  public MoveService(IQueryBus queryBus)
  {
    _queryBus = queryBus;
  }

  public async Task<Move?> ReadAsync(Guid? id, string? key, CancellationToken cancellationToken)
  {
    ReadMoveQuery query = new(id, key);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public Task<SearchResults<Move>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken)
  {
    SearchMovesQuery query = new(payload);
    return _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
