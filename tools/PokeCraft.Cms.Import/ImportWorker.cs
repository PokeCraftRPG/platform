using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Import.Tasks;

namespace PokeCraft.Cms.Import;

internal class ImportWorker : BackgroundService
{
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly PokeApiSettings _pokeApi;

  public ImportWorker(IHostApplicationLifetime hostApplicationLifetime, PokeApiSettings pokeApi)
  {
    _pokeApi = pokeApi;
    _hostApplicationLifetime = hostApplicationLifetime;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    IReadOnlyDictionary<string, Imported<Ability>> abilities = await new ImportAbilitiesTask(_pokeApi).ExecuteAsync(cancellationToken);
    IReadOnlyDictionary<string, Imported<Move>> moves = await new ImportMovesTask(_pokeApi).ExecuteAsync(cancellationToken);
    IReadOnlyDictionary<string, Imported<PokemonSpecies>> species = await new ImportSpeciesTask(_pokeApi).ExecuteAsync(cancellationToken);
    IReadOnlyDictionary<string, Imported<Variety>> varieties = await new ImportVarietiesTask(_pokeApi, species).ExecuteAsync(cancellationToken);

    _hostApplicationLifetime.StopApplication();
  }
}
