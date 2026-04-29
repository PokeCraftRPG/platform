using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportMovesTask
{
  private const string OutputDirectory = "data/moves";

  private readonly ILogger<ImportMovesTask> _logger;
  private readonly PokeApiSettings _pokeApi;

  public ImportMovesTask(ILogger<ImportMovesTask> logger, PokeApiSettings pokeApi)
  {
    _logger = logger;
    _pokeApi = pokeApi;
  }

  public async Task<IReadOnlyDictionary<string, Imported<Move>>> ExecuteAsync(CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<Move> moves = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<Move>> contents = new(capacity: moves.Count);

    foreach (Move move in moves)
    {
      if (!string.IsNullOrWhiteSpace(move.UniqueName))
      {
        ContentPayload? content = Transform(move);
        if (content is not null)
        {
          contents[content.Invariant.UniqueName] = new Imported<Move>(move, content);
          await LoadAsync(content, cancellationToken);
        }
      }
    }

    return contents;
  }

  private async Task<IReadOnlyCollection<Move>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_pokeApi.DataDirectory, "data/api/v2/move");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    List<Move> moves = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, Constants.Encoding, cancellationToken);
      Move? move = ToolsSerializer.Instance.Deserialize<Move>(json);
      if (move is not null)
      {
        moves.Add(move);
      }
    }
    return moves.AsReadOnly();
  }

  private ContentPayload? Transform(Move move)
  {
    string? displayName = move.GetDisplayName(Constants.Language);

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = move.UniqueName;
    content.Invariant.DisplayName = displayName;

    ContentLocalePayload locale = new()
    {
      UniqueName = move.UniqueName,
      DisplayName = displayName,
      Description = move.GetDescription(Constants.Language, Constants.VersionGroup)
    };
    content.Locales[Constants.Language] = locale;

    if (Enum.TryParse(move.Type.UniqueName, ignoreCase: true, out PokemonType type) && Enum.IsDefined(type))
    {
      content.Invariant.FieldValues[nameof(MoveDefinition.Type)] = $"[\"{type}\"]";
    }
    else
    {
      _logger.LogWarning("The move '{Move}' type '{Type}' is not valid.", move, move.Type.UniqueName);
    }

    if (Enum.TryParse(move.Category.UniqueName, ignoreCase: true, out MoveCategory category) && Enum.IsDefined(category))
    {
      content.Invariant.FieldValues[nameof(MoveDefinition.Category)] = $"[\"{category}\"]";
    }
    else
    {
      _logger.LogWarning("The move '{Move}' category '{Category}' is not valid.", move, move.Category.UniqueName);
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
