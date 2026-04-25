using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Species.Models;

namespace PokeCraft.Cms.Core.Species;

public interface ISpeciesQuerier
{
  Task<PokemonSpecies?> ReadAsync(Guid id, bool expanded = false, CancellationToken cancellationToken = default);
  Task<PokemonSpecies?> ReadAsync(string key, bool expanded = false, CancellationToken cancellationToken = default);
  Task<PokemonSpecies?> ReadAsync(int number, bool expanded = false, CancellationToken cancellationToken = default);
  Task<SearchResults<PokemonSpecies>> SearchAsync(SearchSpeciesPayload payload, CancellationToken cancellationToken = default);
}
