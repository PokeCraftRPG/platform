using Logitar.CQRS;
using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportAbilitiesTask : ImportTask
{
  public override string? Description => "Imports PokéAPI Abilities into Krakenar contents.";

  public string Directory { get; }

  public ImportAbilitiesTask(string directory)
  {
    Directory = directory;
  }
}

internal class ImportAbilitiesTaskHandler : ICommandHandler<ImportAbilitiesTask, Unit>
{
  private static readonly Encoding _encoding = Encoding.UTF8;

  private readonly ILogger<ImportAbilitiesTaskHandler> _logger;
  private readonly PokeApiSettings _settings;

  public ImportAbilitiesTaskHandler(ILogger<ImportAbilitiesTaskHandler> logger, PokeApiSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task<Unit> HandleAsync(ImportAbilitiesTask task, CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(task.Directory);

    string directory = Path.Combine(_settings.DataPath, "data/api/v2/ability");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    for (int i = 0; i < paths.Length; i++)
    {
      string path = paths[i];
      string json = await File.ReadAllTextAsync(path, _encoding, cancellationToken);
      PokeApiAbility? ability = ToolsSerializer.Instance.Deserialize<PokeApiAbility>(json);
      if (ability is null)
      {
        _logger.LogWarning("No ability was deserialized from path '{Path}'.", path);
      }
      else if (!string.IsNullOrWhiteSpace(ability.UniqueName))
      {
        ContentPayload content = CreateContent(ability);
        json = ToolsSerializer.Instance.Serialize(content);

        path = Path.Combine(task.Directory, $"{PokemonHelper.Normalize(ability.UniqueName)}.json");
        await File.WriteAllTextAsync(path, json, _encoding, cancellationToken);
      }
    }

    return Unit.Value;
  }

  private static ContentPayload CreateContent(PokeApiAbility ability)
  {
    string? displayName = ability.DisplayNames.SingleOrDefault(x => x.Language.UniqueName == Constants.Language)?.Value;
    string? description = ability.FlavorTextEntries.SingleOrDefault(x => x.Language.UniqueName == Constants.Language && x.VersionGroup.UniqueName == Constants.VersionGroup)?.Value;

    ContentPayload content = new()
    {
      Id = Guid.NewGuid(),
      Invariant = new ContentLocalePayload
      {
        IsPublished = true,
        UniqueName = ability.UniqueName,
        DisplayName = displayName
      }
    };
    content.Locales[Constants.Language] = new ContentLocalePayload
    {
      IsPublished = true,
      UniqueName = ability.UniqueName,
      DisplayName = displayName,
      Description = description
    };
    return content;
  }
}
