using Krakenar.Contracts;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Abilities.Models;

namespace PokeCraft.Cms.Core.Abilities.Queries;

internal record ReadAbilityQuery(Guid? Id, string? Key) : IQuery<Ability?>;

internal class ReadAbilityQueryHandler : IQueryHandler<ReadAbilityQuery, Ability?>
{
  private readonly IAbilityQuerier _abilityQuerier;

  public ReadAbilityQueryHandler(IAbilityQuerier abilityQuerier)
  {
    _abilityQuerier = abilityQuerier;
  }

  public async Task<Ability?> HandleAsync(ReadAbilityQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Ability> abilities = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Ability? ability = await _abilityQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Key))
    {
      Ability? ability = await _abilityQuerier.ReadAsync(query.Key, cancellationToken);
      if (ability is not null)
      {
        abilities[ability.Id] = ability;
      }
    }

    if (abilities.Count > 1)
    {
      throw TooManyResultsException<Ability>.ExpectedSingle(abilities.Count);
    }

    return abilities.Values.SingleOrDefault();
  }
}
