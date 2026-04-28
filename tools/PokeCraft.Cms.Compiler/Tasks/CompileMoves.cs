using Logitar.CQRS;
using PokeCraft.Cms.Compiler.Models;
using PokeCraft.Cms.Compiler.Settings;
using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Compiler.Tasks;

internal class CompileMovesTask : CompilationTask
{
  public override string? Description => "Compiles PokéAPI Moves into Krakenar contents.";

  public string Directory { get; }

  public CompileMovesTask(string directory)
  {
    Directory = directory;
  }
}

internal class CompileMovesTaskHandler : ICommandHandler<CompileMovesTask, Unit>
{
  private static readonly Encoding _encoding = Encoding.UTF8;

  private readonly ILogger<CompileMovesTaskHandler> _logger;
  private readonly PokeApiSettings _settings;

  public CompileMovesTaskHandler(ILogger<CompileMovesTaskHandler> logger, PokeApiSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task<Unit> HandleAsync(CompileMovesTask task, CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(task.Directory);

    string directory = Path.Combine(_settings.DataPath, "data/api/v2/move");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    for (int i = 0; i < paths.Length; i++)
    {
      string path = paths[i];
      string json = await File.ReadAllTextAsync(path, _encoding, cancellationToken);
      PokeApiMove? move = ToolsSerializer.Instance.Deserialize<PokeApiMove>(json);
      if (move is null)
      {
        _logger.LogWarning("No move was deserialized from path '{Path}'.", path);
      }
      else if (!string.IsNullOrWhiteSpace(move.UniqueName) && !move.Type.UniqueName.Equals("shadow", StringComparison.InvariantCultureIgnoreCase))
      {
        // TODO(fpion): would we want to support shadow moves (also without PP)?
        ContentPayload content = CreateContent(move);

        if (Enum.TryParse(move.Type.UniqueName, ignoreCase: true, out PokemonType type))
        {
          content.Invariant.FieldValues[nameof(MoveDefinition.Type)] = $"[\"{type}\"]";
        }
        else
        {
          _logger.LogWarning("The move '{Move}' does not have a valid type '{Type}'.", move, move.Type.UniqueName);
          continue;
        }
        if (Enum.TryParse(move.Category.UniqueName, ignoreCase: true, out MoveCategory category))
        {
          content.Invariant.FieldValues[nameof(MoveDefinition.Category)] = $"[\"{category}\"]";
        }
        else
        {
          _logger.LogWarning("The move '{Move}' does not have a valid category '{Category}'.", move, move.Category.UniqueName);
          continue;
        }

        if (move.Accuracy.HasValue)
        {
          content.Invariant.FieldValues[nameof(MoveDefinition.Accuracy)] = move.Accuracy.Value.ToString();
        }
        if (move.Power.HasValue)
        {
          content.Invariant.FieldValues[nameof(MoveDefinition.Power)] = move.Power.Value.ToString();
        }
        if (move.PowerPoints.HasValue)
        {
          content.Invariant.FieldValues[nameof(MoveDefinition.PowerPoints)] = move.PowerPoints.Value.ToString();
        }
        else
        {
          _logger.LogWarning("The move '{Move}' does not have power points.", move);
          continue;
        }

        path = Path.Combine(task.Directory, $"{PokemonHelper.Normalize(move.UniqueName)}.json");
        json = ToolsSerializer.Instance.Serialize(content);
        await File.WriteAllTextAsync(path, json, _encoding, cancellationToken);
      }
    }

    return Unit.Value;
  }

  private static ContentPayload CreateContent(PokeApiMove move)
  {
    string? displayName = move.DisplayNames.SingleOrDefault(x => x.Language.UniqueName == Constants.Language)?.Value;
    string? description = move.FlavorTextEntries.SingleOrDefault(x => x.Language.UniqueName == Constants.Language && x.VersionGroup.UniqueName == Constants.VersionGroup)?.Value;

    ContentPayload content = new()
    {
      Id = Guid.NewGuid(),
      Invariant = new ContentLocalePayload
      {
        IsPublished = true,
        UniqueName = move.UniqueName,
        DisplayName = displayName
      }
    };
    content.Locales[Constants.Language] = new ContentLocalePayload
    {
      IsPublished = true,
      UniqueName = move.UniqueName,
      DisplayName = displayName,
      Description = description
    };
    return content;
  }
}
