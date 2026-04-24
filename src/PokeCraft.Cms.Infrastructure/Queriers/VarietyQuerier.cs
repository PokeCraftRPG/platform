using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Core.Varieties;
using PokeCraft.Cms.Core.Varieties.Models;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Queriers;

internal class VarietyQuerier : IVarietyQuerier
{
  private readonly IActorService _actors;
  private readonly ISqlHelper _sql;
  private readonly DbSet<VarietyEntity> _varieties;

  public VarietyQuerier(IActorService actors, PokemonContext pokemon, ISqlHelper sql)
  {
    _actors = actors;
    _sql = sql;
    _varieties = pokemon.Varieties;
  }

  public async Task<Variety?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _varieties.AsNoTracking().Include(x => x.Species)
      .Where(x => x.UniqueId == id && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return variety is null ? null : await MapAsync(variety, cancellationToken);
  }
  public async Task<Variety?> ReadAsync(string key, CancellationToken cancellationToken)
  {
    VarietyEntity? variety = await _varieties.AsNoTracking().Include(x => x.Species)
      .Where(x => x.Key == PokemonHelper.Normalize(key) && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return variety is null ? null : await MapAsync(variety, cancellationToken);
  }

  public async Task<SearchResults<Variety>> SearchAsync(SearchVarietiesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sql.Query(PokemonDb.Varieties.Table).SelectAll(PokemonDb.Varieties.Table)
      .Join(PokemonDb.Species.SpeciesId, PokemonDb.Varieties.SpeciesId, new OperatorCondition(PokemonDb.Species.IsPublished, Operators.IsEqualTo(true)))
      .Where(PokemonDb.Varieties.IsPublished, Operators.IsEqualTo(true))
      .ApplyIdFilter(PokemonDb.Varieties.UniqueId, payload.Ids);
    _sql.ApplyTextSearch(builder, payload.Search, PokemonDb.Varieties.Key, PokemonDb.Varieties.Name, PokemonDb.Varieties.Genus);

    if (payload.SpeciesId.HasValue)
    {
      builder.Where(PokemonDb.Species.UniqueId, Operators.IsEqualTo(payload.SpeciesId.Value));
    }
    if (payload.IsDefault.HasValue)
    {
      builder.Where(PokemonDb.Varieties.IsDefault, Operators.IsEqualTo(payload.IsDefault.Value));
    }
    if (payload.CanChangeForm.HasValue)
    {
      builder.Where(PokemonDb.Varieties.CanChangeForm, Operators.IsEqualTo(payload.CanChangeForm.Value));
    }

    IQueryable<VarietyEntity> query = _varieties.FromQuery(builder).AsNoTracking()
      .Include(x => x.Species);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<VarietyEntity>? ordered = null;
    foreach (VarietySortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case VarietySort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case VarietySort.Key:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Key) : ordered.ThenBy(x => x.Key));
          break;
        case VarietySort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case VarietySort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    VarietyEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<Variety> varieties = await MapAsync(entities, cancellationToken);

    return new SearchResults<Variety>(varieties, total);
  }

  private async Task<Variety> MapAsync(VarietyEntity variety, CancellationToken cancellationToken)
  {
    return (await MapAsync([variety], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<Variety>> MapAsync(IEnumerable<VarietyEntity> varieties, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = varieties.SelectMany(x => x.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actors.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return varieties.Select(mapper.ToVariety).ToList().AsReadOnly();
  }
}
