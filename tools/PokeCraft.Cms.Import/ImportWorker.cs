using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Tasks;

namespace PokeCraft.Cms.Import;

internal class ImportWorker : BackgroundService
{
  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly IServiceProvider _serviceProvider;

  public ImportWorker(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
    _hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    ImportAbilitiesTask importAbilities = _serviceProvider.GetRequiredService<ImportAbilitiesTask>();
    IReadOnlyDictionary<string, Imported<Ability>> abilities = await importAbilities.ExecuteAsync(cancellationToken);

    ImportMovesTask importMoves = _serviceProvider.GetRequiredService<ImportMovesTask>();
    IReadOnlyDictionary<string, Imported<Move>> moves = await importMoves.ExecuteAsync(cancellationToken);

    ImportSpeciesTask importSpecies = _serviceProvider.GetRequiredService<ImportSpeciesTask>();
    IReadOnlyDictionary<string, Imported<PokemonSpecies>> species = await importSpecies.ExecuteAsync(cancellationToken);

    ImportVarietiesTask importVarieties = _serviceProvider.GetRequiredService<ImportVarietiesTask>();
    IReadOnlyDictionary<string, Imported<Variety>> varieties = await importVarieties.ExecuteAsync(species, cancellationToken);

    ImportFormsTask importForms = _serviceProvider.GetRequiredService<ImportFormsTask>();
    IReadOnlyDictionary<string, Imported<Form>> forms = await importForms.ExecuteAsync(abilities, species, varieties, cancellationToken);

    _hostApplicationLifetime.StopApplication();
  }
}
