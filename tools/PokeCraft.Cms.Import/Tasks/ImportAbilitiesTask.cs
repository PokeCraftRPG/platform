using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportAbilitiesTask
{
  private const string OutputDirectory = "data/abilities";

  private readonly PokeApiSettings _pokeApi;

  public ImportAbilitiesTask(PokeApiSettings pokeApi)
  {
    _pokeApi = pokeApi;
  }

  public async Task<IReadOnlyDictionary<string, Imported<Ability>>> ExecuteAsync(CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<Ability> abilities = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<Ability>> contents = new(capacity: abilities.Count);

    foreach (Ability ability in abilities)
    {
      if (!string.IsNullOrWhiteSpace(ability.UniqueName))
      {
        ContentPayload content = Transform(ability);
        contents[content.Invariant.UniqueName] = new Imported<Ability>(ability, content);
        await LoadAsync(content, cancellationToken);
      }
    }

    return contents.AsReadOnly();
  }

  private async Task<IReadOnlyCollection<Ability>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_pokeApi.DataDirectory, "data/api/v2/ability");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    List<Ability> abilities = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, Constants.Encoding, cancellationToken);
      Ability? ability = ToolsSerializer.Instance.Deserialize<Ability>(json);
      if (ability is not null)
      {
        abilities.Add(ability);
      }
    }
    return abilities.AsReadOnly();
  }

  private static ContentPayload Transform(Ability ability)
  {
    string? displayName = ability.GetDisplayName(Constants.Language);

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = ability.UniqueName;
    content.Invariant.DisplayName = displayName;

    ContentLocalePayload locale = new()
    {
      UniqueName = ability.UniqueName,
      DisplayName = displayName,
      Description = ability.GetDescription(Constants.Language, Constants.VersionGroup)
    };
    content.Locales[Constants.Language] = locale;

    return content;
  }

  private static async Task LoadAsync(ContentPayload content, CancellationToken cancellationToken)
  {
    string path = Path.Combine(OutputDirectory, $"{content.Invariant.UniqueName}.json");
    string json = ToolsSerializer.Instance.Serialize(content);
    await File.WriteAllTextAsync(path, json, Constants.Encoding, cancellationToken);
  }
}
