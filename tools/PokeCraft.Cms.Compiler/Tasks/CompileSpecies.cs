using Logitar.CQRS;
using PokeCraft.Cms.Compiler.Models;
using PokeCraft.Cms.Compiler.Settings;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Infrastructure;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Compiler.Tasks;

internal class CompileSpeciesTask : CompilationTask
{
  public override string? Description => "Compiles PokéAPI Species into Krakenar contents.";

  public string Directory { get; }

  public CompileSpeciesTask(string directory)
  {
    Directory = directory;
  }
}

internal class CompileSpeciesTaskHandler : ICommandHandler<CompileSpeciesTask, Unit>
{
  private static readonly Encoding _encoding = Encoding.UTF8;

  private readonly ILogger<CompileSpeciesTaskHandler> _logger;
  private readonly PokeApiSettings _settings;

  public CompileSpeciesTaskHandler(ILogger<CompileSpeciesTaskHandler> logger, PokeApiSettings settings)
  {
    _logger = logger;
    _settings = settings;
  }

  public async Task<Unit> HandleAsync(CompileSpeciesTask task, CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(task.Directory);

    string directory = Path.Combine(_settings.DataPath, "data/api/v2/pokemon-species");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    for (int i = 0; i < paths.Length; i++)
    {
      string path = paths[i];
      string json = await File.ReadAllTextAsync(path, _encoding, cancellationToken);
      PokeApiSpecies? species = ToolsSerializer.Instance.Deserialize<PokeApiSpecies>(json);
      if (species is null)
      {
        _logger.LogWarning("No species was deserialized from path '{Path}'.", path);
      }
      else if (!string.IsNullOrWhiteSpace(species.UniqueName))
      {
        ContentPayload content = CreateContent(species);

        int? number = species.PokedexNumbers.SingleOrDefault(x => x.Pokedex.UniqueName == Constants.NationalPokedex)?.Number;
        if (number.HasValue)
        {
          content.Invariant.FieldValues[nameof(SpeciesDefinition.Number)] = number.Value.ToString();
        }
        else
        {
          _logger.LogWarning("The species '{Species}' does not have a national Pokédex number.", species);
          continue;
        }

        SpeciesCategory? category = Categorize(species);
        if (category.HasValue)
        {
          content.Invariant.FieldValues[nameof(SpeciesDefinition.Category)] = $"[\"{category}\"]";
        }
        else
        {
          _logger.LogWarning("The species '{Species}' does not have a valid category.", species);
          continue;
        }

        content.Invariant.FieldValues[nameof(SpeciesDefinition.BaseFriendship)] = species.BaseFriendship.ToString();
        content.Invariant.FieldValues[nameof(SpeciesDefinition.CatchRate)] = species.CatchRate.ToString();

        GrowthRate? growthRate = ParseGrowthRate(species);
        if (growthRate.HasValue)
        {
          content.Invariant.FieldValues[nameof(SpeciesDefinition.GrowthRate)] = $"[\"{growthRate}\"]";
        }
        else
        {
          _logger.LogWarning("The species '{Species}' does not have a valid growth rate.", species);
          continue;
        }

        content.Invariant.FieldValues[nameof(SpeciesDefinition.EggCycles)] = species.EggCycles.ToString();

        if (species.EggGroups.Count < 1)
        {
          _logger.LogWarning("The species '{Species}' does not have any egg group.", species);
          continue;
        }
        else if (species.EggGroups.Count > 2)
        {
          _logger.LogWarning("The species '{Species}' has too many egg groups ({Count}).", species, species.EggGroups.Count);
          continue;
        }
        else
        {
          EggGroup? eggGroup = ParseEggGroup(species.EggGroups.First().UniqueName);
          if (eggGroup.HasValue)
          {
            content.Invariant.FieldValues[nameof(SpeciesDefinition.PrimaryEggGroup)] = $"[\"{eggGroup}\"]";
          }
          else
          {
            _logger.LogWarning("The species '{Species}' does not have a valid primary egg group.", species);
            continue;
          }

          if (species.EggGroups.Count == 2)
          {
            eggGroup = ParseEggGroup(species.EggGroups.Last().UniqueName);
            if (eggGroup.HasValue)
            {
              content.Invariant.FieldValues[nameof(SpeciesDefinition.SecondaryEggGroup)] = $"[\"{eggGroup}\"]";
            }
            else
            {
              _logger.LogWarning("The species '{Species}' does not have a valid secondary egg group.", species);
              continue;
            }
          }
        }

        path = Path.Combine(task.Directory, $"{PokemonHelper.Normalize(species.UniqueName)}.json");
        json = ToolsSerializer.Instance.Serialize(content);
        await File.WriteAllTextAsync(path, json, _encoding, cancellationToken);
      }
    }

    return Unit.Value;
  }

  private static SpeciesCategory? Categorize(PokeApiSpecies species)
  {
    List<SpeciesCategory> categories = new(capacity: 3);
    if (species.IsBaby)
    {
      categories.Add(SpeciesCategory.Baby);
    }
    if (species.IsLegendary)
    {
      categories.Add(SpeciesCategory.Legendary);
    }
    if (species.IsMythical)
    {
      categories.Add(SpeciesCategory.Mythical);
    }

    if (categories.Count < 1)
    {
      return SpeciesCategory.Standard;
    }
    else if (categories.Count > 1)
    {
      return null;
    }
    else
    {
      return categories.Single();
    }
  }

  private static ContentPayload CreateContent(PokeApiSpecies species)
  {
    string? displayName = species.DisplayNames.SingleOrDefault(x => x.Language.UniqueName == Constants.Language)?.Value;

    ContentPayload content = new()
    {
      Id = Guid.NewGuid(),
      Invariant = new ContentLocalePayload
      {
        IsPublished = true,
        UniqueName = species.UniqueName,
        DisplayName = displayName
      }
    };
    content.Locales[Constants.Language] = new ContentLocalePayload
    {
      IsPublished = true,
      UniqueName = species.UniqueName,
      DisplayName = displayName
    };
    return content;
  }

  private static EggGroup? ParseEggGroup(string value)
  {
    value = PokemonHelper.Normalize(value);
    return value switch
    {
      "bug" => EggGroup.Bug,
      "ditto" => EggGroup.Ditto,
      "dragon" => EggGroup.Dragon,
      "fairy" => EggGroup.Fairy,
      "flying" => EggGroup.Flying,
      "ground" => EggGroup.Field,
      "humanshape" => EggGroup.HumanLike,
      "indeterminate" => EggGroup.Amorphous,
      "mineral" => EggGroup.Mineral,
      "monster" => EggGroup.Monster,
      "no-eggs" => EggGroup.NoEggsDiscovered,
      "plant" => EggGroup.Grass,
      "water1" => EggGroup.Water1,
      "water2" => EggGroup.Water2,
      "water3" => EggGroup.Water3,
      _ => null,
    };
  }

  private static GrowthRate? ParseGrowthRate(PokeApiSpecies species) => PokemonHelper.Normalize(species.GrowthRate.UniqueName) switch
  {
    "fast" => GrowthRate.Fast,
    "fast-then-very-slow" => GrowthRate.Fluctuating,
    "medium" => GrowthRate.MediumFast,
    "medium-slow" => GrowthRate.MediumSlow,
    "slow" => GrowthRate.Slow,
    "slow-then-very-fast" => GrowthRate.Erratic,
    _ => null,
  };
}
