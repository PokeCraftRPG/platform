using Logitar.CQRS;
using PokeCraft.Cms.Compiler.Models;
using PokeCraft.Cms.Compiler.Settings;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Compiler.Tasks;

internal class CompileAbilitiesTask : CompilationTask
{
  public override string? Description => "Compiles PokéAPI Abilities into Krakenar contents.";

  public string Directory { get; }

  public CompileAbilitiesTask(string directory)
  {
    Directory = directory;
  }
}

internal class CompileAbilitiesTaskHandler : ICommandHandler<CompileAbilitiesTask, Unit>
{
  private static readonly Encoding _encoding = Encoding.UTF8;

  private readonly ILogger<CompileAbilitiesTaskHandler> _logger;
  private readonly PokeApiSettings _settings;

  public CompileAbilitiesTaskHandler(ILogger<CompileAbilitiesTaskHandler> logger, PokeApiSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task<Unit> HandleAsync(CompileAbilitiesTask task, CancellationToken cancellationToken)
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
