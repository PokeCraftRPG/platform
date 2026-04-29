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

  private readonly PokeApiSettings _pokeApi;
  private readonly IReadOnlyDictionary<string, Imported<PokemonSpecies>> _speciesContents;

  public ImportVarietiesTask(PokeApiSettings pokeApi, IReadOnlyDictionary<string, Imported<PokemonSpecies>> species)
  {
    _pokeApi = pokeApi;
    _speciesContents = species;
  }

  public async Task<IReadOnlyDictionary<string, Imported<Variety>>> ExecuteAsync(CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<Variety> varieties = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<Variety>> contents = new(capacity: varieties.Count);

    foreach (Variety variety in varieties)
    {
      if (!string.IsNullOrWhiteSpace(variety.UniqueName))
      {
        ContentPayload? content = Transform(variety);
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

  private ContentPayload? Transform(Variety variety)
  {
    if (!_speciesContents.TryGetValue(variety.Species.UniqueName, out Imported<PokemonSpecies>? species))
    {
      return null;
    }

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = variety.UniqueName;

    content.Invariant.FieldValues[nameof(VarietyDefinition.Species)] = $"[\"{species.Content.Id}\"]";
    content.Invariant.FieldValues[nameof(VarietyDefinition.IsDefault)] = variety.IsDefault.ToString();

    if (species.Entity.GenderRatio >= 0 && species.Entity.GenderRatio <= 8)
    {
      content.Invariant.FieldValues[nameof(VarietyDefinition.GenderRatio)] = (8 - species.Entity.GenderRatio).ToString();
    }
    content.Invariant.FieldValues[nameof(VarietyDefinition.CanChangeForm)] = species.Entity.CanChangeForm.ToString();

    ContentLocalePayload locale = new()
    {
      UniqueName = variety.UniqueName
    };
    content.Locales[Constants.Language] = locale;

    Genus[] genera = species.Entity.Genera.Where(x => x.Language.UniqueName == Constants.Language).ToArray();
    if (genera.Length == 1)
    {
      locale.FieldValues[nameof(VarietyDefinition.Genus)] = genera[0].Value.Remove(" Pokémon");
    }

    return content;
  }

  private static async Task LoadAsync(ContentPayload content, CancellationToken cancellationToken)
  {
    string path = Path.Combine(OutputDirectory, $"{content.Invariant.UniqueName}.json");
    string json = ToolsSerializer.Instance.Serialize(content);
    await File.WriteAllTextAsync(path, json, Constants.Encoding, cancellationToken);
  }
}
