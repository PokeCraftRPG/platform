using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Core.Species;

public interface ISpeciesQuerier
{
  Task<PokemonSpecies?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PokemonSpecies?> ReadAsync(string key, CancellationToken cancellationToken = default);
  Task<PokemonSpecies?> ReadAsync(int number, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonSpecies>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
}
