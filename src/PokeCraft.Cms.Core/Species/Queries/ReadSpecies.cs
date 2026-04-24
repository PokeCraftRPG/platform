using Krakenar.Contracts;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Core.Species.Queries;

internal record ReadSpeciesQuery(Guid? Id, string? Key, int? Number) : IQuery<PokemonSpecies?>;

internal class ReadSpeciesQueryHandler : IQueryHandler<ReadSpeciesQuery, PokemonSpecies?>
{
  private readonly ISpeciesQuerier _speciesQuerier;

  public ReadSpeciesQueryHandler(ISpeciesQuerier speciesQuerier)
  {
    _speciesQuerier = speciesQuerier;
  }

  public async Task<PokemonSpecies?> HandleAsync(ReadSpeciesQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, PokemonSpecies> species = new(capacity: 3);

    if (query.Id.HasValue)
    {
      PokemonSpecies? item = await _speciesQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (item is not null)
      {
        species[item.Id] = item;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Key))
    {
      PokemonSpecies? item = await _speciesQuerier.ReadAsync(query.Key, cancellationToken);
      if (item is not null)
      {
        species[item.Id] = item;
      }
    }

    if (query.Number.HasValue)
    {
      PokemonSpecies? item = await _speciesQuerier.ReadAsync(query.Number.Value, cancellationToken);
      if (item is not null)
      {
        species[item.Id] = item;
      }
    }

    if (species.Count > 1)
    {
      throw TooManyResultsException<PokemonSpecies>.ExpectedSingle(species.Count);
    }

    return species.Values.SingleOrDefault();
  }
}
