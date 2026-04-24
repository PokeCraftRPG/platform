using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Moves.Models;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Queriers;

internal class MoveQuerier : IMoveQuerier
{
  private readonly IActorService _actors;
  private readonly DbSet<MoveEntity> _moves;
  private readonly ISqlHelper _sql;

  public MoveQuerier(IActorService actors, PokemonContext pokemon, ISqlHelper sql)
  {
    _actors = actors;
    _moves = pokemon.Moves;
    _sql = sql;
  }

  public async Task<Move?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _moves.AsNoTracking()
      .Where(x => x.UniqueId == id && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return move is null ? null : await MapAsync(move, cancellationToken);
  }
  public async Task<Move?> ReadAsync(string key, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _moves.AsNoTracking()
      .Where(x => x.Key == PokemonHelper.Normalize(key) && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return move is null ? null : await MapAsync(move, cancellationToken);
  }

  public async Task<SearchResults<Move>> SearchAsync(SearchMovesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sql.Query(PokemonDb.Moves.Table).SelectAll(PokemonDb.Moves.Table)
      .Where(PokemonDb.Moves.IsPublished, Operators.IsEqualTo(true))
      .ApplyIdFilter(PokemonDb.Moves.UniqueId, payload.Ids);
    _sql.ApplyTextSearch(builder, payload.Search, PokemonDb.Moves.Key, PokemonDb.Moves.Name);

    if (payload.Type.HasValue)
    {
      builder.Where(PokemonDb.Moves.Type, Operators.IsEqualTo(payload.Type.Value.ToString()));
    }
    if (payload.Category.HasValue)
    {
      builder.Where(PokemonDb.Moves.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }

    IQueryable<MoveEntity> query = _moves.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<MoveEntity>? ordered = null;
    foreach (MoveSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case MoveSort.Accuracy:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Accuracy) : query.OrderBy(x => x.Accuracy))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Accuracy) : ordered.ThenBy(x => x.Accuracy));
          break;
        case MoveSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case MoveSort.Key:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Key) : ordered.ThenBy(x => x.Key));
          break;
        case MoveSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case MoveSort.Power:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Power) : query.OrderBy(x => x.Power))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Power) : ordered.ThenBy(x => x.Power));
          break;
        case MoveSort.PowerPoints:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.PowerPoints) : query.OrderBy(x => x.PowerPoints))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.PowerPoints) : ordered.ThenBy(x => x.PowerPoints));
          break;
        case MoveSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    MoveEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<Move> moves = await MapAsync(entities, cancellationToken);

    return new SearchResults<Move>(moves, total);
  }

  private async Task<Move> MapAsync(MoveEntity move, CancellationToken cancellationToken)
  {
    return (await MapAsync([move], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<Move>> MapAsync(IEnumerable<MoveEntity> moves, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = moves.SelectMany(move => move.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actors.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return moves.Select(mapper.ToMove).ToList().AsReadOnly();
  }
}
