using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Core.Species.Models;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Queriers;

internal class SpeciesQuerier : ISpeciesQuerier
{
  private readonly IActorService _actors;
  private readonly DbSet<SpeciesEntity> _species;
  private readonly ISqlHelper _sql;

  public SpeciesQuerier(IActorService actors, PokemonContext pokemon, ISqlHelper sql)
  {
    _actors = actors;
    _species = pokemon.Species;
    _sql = sql;
  }

  public async Task<PokemonSpecies?> ReadAsync(Guid id, bool expand, CancellationToken cancellationToken)
  {
    IQueryable<SpeciesEntity> query = _species.AsNoTracking().Where(x => x.UniqueId == id && x.IsPublished);
    if (expand)
    {
      query = query.AsSplitQuery()
        .Include(x => x.Varieties).ThenInclude(x => x.Forms).ThenInclude(x => x.Abilities).ThenInclude(x => x.Ability)
        .Include(x => x.Varieties).ThenInclude(x => x.Moves).ThenInclude(x => x.Move);
    }
    SpeciesEntity? species = await query.SingleOrDefaultAsync(cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<PokemonSpecies?> ReadAsync(string key, bool expand, CancellationToken cancellationToken)
  {
    IQueryable<SpeciesEntity> query = _species.AsNoTracking().Where(x => x.Key == PokemonHelper.Normalize(key) && x.IsPublished);
    if (expand)
    {
      query = query.AsSplitQuery()
        .Include(x => x.Varieties).ThenInclude(x => x.Forms).ThenInclude(x => x.Abilities).ThenInclude(x => x.Ability)
        .Include(x => x.Varieties).ThenInclude(x => x.Moves).ThenInclude(x => x.Move);
    }
    SpeciesEntity? species = await query.SingleOrDefaultAsync(cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }
  public async Task<PokemonSpecies?> ReadAsync(int number, bool expand, CancellationToken cancellationToken)
  {
    IQueryable<SpeciesEntity> query = _species.AsNoTracking().Where(x => x.Number == number && x.IsPublished);
    if (expand)
    {
      query = query.AsSplitQuery()
        .Include(x => x.Varieties).ThenInclude(x => x.Forms).ThenInclude(x => x.Abilities).ThenInclude(x => x.Ability)
        .Include(x => x.Varieties).ThenInclude(x => x.Moves).ThenInclude(x => x.Move);
    }
    SpeciesEntity? species = await query.SingleOrDefaultAsync(cancellationToken);
    return species is null ? null : await MapAsync(species, cancellationToken);
  }

  public async Task<SearchResults<PokemonSpecies>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sql.Query(PokemonDb.Species.Table).SelectAll(PokemonDb.Species.Table)
      .Where(PokemonDb.Species.IsPublished, Operators.IsEqualTo(true))
      .ApplyIdFilter(PokemonDb.Species.UniqueId, payload.Ids);
    _sql.ApplyTextSearch(builder, payload.Search, PokemonDb.Species.Key, PokemonDb.Species.Name);

    if (payload.Category.HasValue)
    {
      builder.Where(PokemonDb.Species.Category, Operators.IsEqualTo(payload.Category.Value.ToString()));
    }
    if (payload.GrowthRate.HasValue)
    {
      builder.Where(PokemonDb.Species.GrowthRate, Operators.IsEqualTo(payload.GrowthRate.Value.ToString()));
    }
    if (payload.EggGroup.HasValue)
    {
      builder.WhereOr(
        new OperatorCondition(PokemonDb.Species.PrimaryEggGroup, Operators.IsEqualTo(payload.EggGroup.Value.ToString())),
        new OperatorCondition(PokemonDb.Species.SecondaryEggGroup, Operators.IsEqualTo(payload.EggGroup.Value.ToString())));
    }

    IQueryable<SpeciesEntity> query = _species.FromQuery(builder).AsNoTracking();

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<SpeciesEntity>? ordered = null;
    foreach (SpeciesSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case SpeciesSort.BaseFriendship:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.BaseFriendship) : query.OrderBy(x => x.BaseFriendship))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.BaseFriendship) : ordered.ThenBy(x => x.BaseFriendship));
          break;
        case SpeciesSort.CatchRate:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CatchRate) : query.OrderBy(x => x.CatchRate))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CatchRate) : ordered.ThenBy(x => x.CatchRate));
          break;
        case SpeciesSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case SpeciesSort.EggCycles:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.EggCycles) : query.OrderBy(x => x.EggCycles))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.EggCycles) : ordered.ThenBy(x => x.EggCycles));
          break;
        case SpeciesSort.Key:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Key) : ordered.ThenBy(x => x.Key));
          break;
        case SpeciesSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case SpeciesSort.Number:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Number) : query.OrderBy(x => x.Number))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Number) : ordered.ThenBy(x => x.Number));
          break;
        case SpeciesSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    SpeciesEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<PokemonSpecies> species = await MapAsync(entities, cancellationToken);

    return new SearchResults<PokemonSpecies>(species, total);
  }

  private async Task<PokemonSpecies> MapAsync(SpeciesEntity species, CancellationToken cancellationToken)
  {
    return (await MapAsync([species], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<PokemonSpecies>> MapAsync(IEnumerable<SpeciesEntity> species, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = species.SelectMany(x => x.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actors.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return species.Select(mapper.ToSpecies).ToList().AsReadOnly();
  }
}
