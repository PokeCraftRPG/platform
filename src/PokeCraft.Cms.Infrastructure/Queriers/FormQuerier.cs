using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Search;
using Krakenar.Core.Actors;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.Data;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Core.Forms;
using PokeCraft.Cms.Core.Forms.Models;
using PokeCraft.Cms.Infrastructure.Entities;

namespace PokeCraft.Cms.Infrastructure.Queriers;

internal class FormQuerier : IFormQuerier
{
  private readonly IActorService _actors;
  private readonly DbSet<FormEntity> _forms;
  private readonly ISqlHelper _sql;

  public FormQuerier(IActorService actors, PokemonContext pokemon, ISqlHelper sql)
  {
    _actors = actors;
    _forms = pokemon.Forms;
    _sql = sql;
  }

  public async Task<Form?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FormEntity? form = await _forms.AsNoTracking().Include(x => x.Variety!).ThenInclude(x => x.Species)
      .Where(x => x.UniqueId == id && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return form is null ? null : await MapAsync(form, cancellationToken);
  }
  public async Task<Form?> ReadAsync(string key, CancellationToken cancellationToken)
  {
    FormEntity? form = await _forms.AsNoTracking().Include(x => x.Variety!).ThenInclude(x => x.Species)
      .Where(x => x.Key == PokemonHelper.Normalize(key) && x.IsPublished)
      .SingleOrDefaultAsync(cancellationToken);
    return form is null ? null : await MapAsync(form, cancellationToken);
  }

  public async Task<SearchResults<Form>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken)
  {
    IQueryBuilder builder = _sql.Query(PokemonDb.Forms.Table).SelectAll(PokemonDb.Forms.Table)
      .Where(PokemonDb.Forms.IsPublished, Operators.IsEqualTo(true))
      .ApplyIdFilter(PokemonDb.Forms.UniqueId, payload.Ids);
    _sql.ApplyTextSearch(builder, payload.Search, PokemonDb.Forms.Key, PokemonDb.Forms.Name);

    if (payload.VarietyId.HasValue)
    {
      OperatorCondition condition = new(PokemonDb.Varieties.UniqueId, Operators.IsEqualTo(payload.VarietyId.Value));
      builder.Join(PokemonDb.Varieties.VarietyId, PokemonDb.Forms.VarietyId, condition);
    }
    if (payload.IsDefault.HasValue)
    {
      builder.Where(PokemonDb.Forms.IsDefault, Operators.IsEqualTo(payload.IsDefault.Value));
    }
    if (payload.HasGenderDifferences.HasValue)
    {
      builder.Where(PokemonDb.Forms.HasGenderDifferences, Operators.IsEqualTo(payload.HasGenderDifferences.Value));
    }
    if (payload.IsBattleOnly.HasValue)
    {
      builder.Where(PokemonDb.Forms.IsBattleOnly, Operators.IsEqualTo(payload.IsBattleOnly.Value));
    }
    if (payload.IsMega.HasValue)
    {
      builder.Where(PokemonDb.Forms.IsMega, Operators.IsEqualTo(payload.IsMega.Value));
    }
    if (payload.Type.HasValue)
    {
      builder.WhereOr(
        new OperatorCondition(PokemonDb.Forms.PrimaryType, Operators.IsEqualTo(payload.Type.Value.ToString())),
        new OperatorCondition(PokemonDb.Forms.SecondaryType, Operators.IsEqualTo(payload.Type.Value.ToString())));
    }

    IQueryable<FormEntity> query = _forms.FromQuery(builder).AsNoTracking()
      .Include(x => x.Abilities).ThenInclude(x => x.Ability)
      .Include(x => x.Variety).ThenInclude(x => x!.Species);

    long total = await query.LongCountAsync(cancellationToken);

    IOrderedQueryable<FormEntity>? ordered = null;
    foreach (FormSortOption sort in payload.Sort)
    {
      switch (sort.Field)
      {
        case FormSort.CreatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.CreatedOn) : query.OrderBy(x => x.CreatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.CreatedOn) : ordered.ThenBy(x => x.CreatedOn));
          break;
        case FormSort.Height:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Height) : query.OrderBy(x => x.Height))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Height) : ordered.ThenBy(x => x.Height));
          break;
        case FormSort.Key:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Key) : query.OrderBy(x => x.Key))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Key) : ordered.ThenBy(x => x.Key));
          break;
        case FormSort.Name:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Name) : ordered.ThenBy(x => x.Name));
          break;
        case FormSort.UpdatedOn:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.UpdatedOn) : query.OrderBy(x => x.UpdatedOn))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.UpdatedOn) : ordered.ThenBy(x => x.UpdatedOn));
          break;
        case FormSort.Weight:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.Weight) : query.OrderBy(x => x.Weight))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.Weight) : ordered.ThenBy(x => x.Weight));
          break;
        case FormSort.YieldExperience:
          ordered = (ordered is null)
            ? (sort.IsDescending ? query.OrderByDescending(x => x.YieldExperience) : query.OrderBy(x => x.YieldExperience))
            : (sort.IsDescending ? ordered.ThenByDescending(x => x.YieldExperience) : ordered.ThenBy(x => x.YieldExperience));
          break;
      }
    }
    query = ordered ?? query;

    query = query.ApplyPaging(payload);

    FormEntity[] entities = await query.ToArrayAsync(cancellationToken);
    IReadOnlyCollection<Form> forms = await MapAsync(entities, cancellationToken);

    return new SearchResults<Form>(forms, total);
  }

  private async Task<Form> MapAsync(FormEntity form, CancellationToken cancellationToken)
  {
    return (await MapAsync([form], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<Form>> MapAsync(IEnumerable<FormEntity> forms, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = forms.SelectMany(x => x.GetActorIds());
    IReadOnlyDictionary<ActorId, Actor> actors = await _actors.FindAsync(actorIds, cancellationToken);
    PokemonMapper mapper = new(actors);

    return forms.Select(mapper.ToForm).ToList().AsReadOnly();
  }
}
