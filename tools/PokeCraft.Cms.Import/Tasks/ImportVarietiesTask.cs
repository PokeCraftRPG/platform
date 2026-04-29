using Logitar;
using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportVarietiesTask
{
  private const string OutputDirectory = "data/varieties";

  private readonly ILogger<ImportVarietiesTask> _logger;
  private readonly PokeApiSettings _pokeApi;

  public ImportVarietiesTask(ILogger<ImportVarietiesTask> logger, PokeApiSettings pokeApi)
  {
    _logger = logger;
    _pokeApi = pokeApi;
  }

  public async Task<IReadOnlyDictionary<string, Imported<Variety>>> ExecuteAsync(
    IReadOnlyDictionary<string, Imported<PokemonSpecies>> species,
    CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<Variety> varieties = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<Variety>> contents = new(capacity: varieties.Count);

    foreach (Variety variety in varieties)
    {
      if (!string.IsNullOrWhiteSpace(variety.UniqueName))
      {
        ContentPayload? content = Transform(variety, species);
        if (content is not null)
        {
          contents[content.Invariant.UniqueName] = new Imported<Variety>(variety, content);
          await LoadAsync(content, cancellationToken);
        }
      }
    }

    return contents.AsReadOnly();
  }

  private async Task<IReadOnlyCollection<Variety>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_pokeApi.DataDirectory, "data/api/v2/pokemon");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    List<Variety> varieties = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      if (!path.Contains("encounters"))
      {
        string json = await File.ReadAllTextAsync(path, Constants.Encoding, cancellationToken);
        Variety? variety = ToolsSerializer.Instance.Deserialize<Variety>(json);
        if (variety is not null)
        {
          varieties.Add(variety);
        }
      }
    }
    return varieties.AsReadOnly();
  }

  private ContentPayload? Transform(Variety variety, IReadOnlyDictionary<string, Imported<PokemonSpecies>> importedSpecies)
  {
    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = variety.UniqueName;

    ContentLocalePayload locale = new()
    {
      UniqueName = variety.UniqueName
    };
    content.Locales[Constants.Language] = locale;

    if (importedSpecies.TryGetValue(variety.Species.UniqueName, out Imported<PokemonSpecies>? species))
    {
      content.Invariant.FieldValues[nameof(VarietyDefinition.Species)] = $"[\"{species.Content.Id}\"]";

      if (species.Entity.GenderRatio >= 0 && species.Entity.GenderRatio <= 8)
      {
        content.Invariant.FieldValues[nameof(VarietyDefinition.GenderRatio)] = (8 - species.Entity.GenderRatio).ToString();
      }
      else if (species.Entity.GenderRatio != -1)
      {
        _logger.LogWarning("The Pokémon variety '{Variety}' gender ratio '{GenderRatio}' is not valid.", variety, species.Entity.GenderRatio);
      }

      content.Invariant.FieldValues[nameof(VarietyDefinition.CanChangeForm)] = species.Entity.CanChangeForm.ToString();

      Genus[] genera = species.Entity.Genera.Where(x => x.Language.UniqueName == Constants.Language).ToArray();
      if (genera.Length == 1)
      {
        locale.FieldValues[nameof(VarietyDefinition.Genus)] = genera[0].Value.Remove(" Pokémon");
      }
      else
      {
        _logger.LogWarning("The Pokémon variety '{Variety}' must have exactly 1 genus (Count: {Count}).", variety, genera.Length);
      }
    }
    else
    {
      _logger.LogWarning("The Pokémon variety '{Variety}' species '{Species}' was not found.", variety, variety.Species.UniqueName);
    }

    content.Invariant.FieldValues[nameof(VarietyDefinition.IsDefault)] = variety.IsDefault.ToString();

    return content;
  }

  private static async Task LoadAsync(ContentPayload content, CancellationToken cancellationToken)
  {
    string path = Path.Combine(OutputDirectory, $"{content.Invariant.UniqueName}.json");
    string json = ToolsSerializer.Instance.Serialize(content);
    await File.WriteAllTextAsync(path, json, Constants.Encoding, cancellationToken);
  }
}
