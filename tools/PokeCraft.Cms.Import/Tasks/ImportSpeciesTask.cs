using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportSpeciesTask
{
  private const string OutputDirectory = "data/species";

  private readonly ILogger<ImportSpeciesTask> _logger;
  private readonly PokeApiSettings _pokeApi;

  public ImportSpeciesTask(ILogger<ImportSpeciesTask> logger, PokeApiSettings pokeApi)
  {
    _logger = logger;
    _pokeApi = pokeApi;
  }

  public async Task<IReadOnlyDictionary<string, Imported<PokemonSpecies>>> ExecuteAsync(CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<PokemonSpecies> speciesList = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<PokemonSpecies>> contents = new(capacity: speciesList.Count);

    foreach (PokemonSpecies species in speciesList)
    {
      if (!string.IsNullOrWhiteSpace(species.UniqueName))
      {
        ContentPayload? content = Transform(species);
        if (content is not null)
        {
          contents[content.Invariant.UniqueName] = new Imported<PokemonSpecies>(species, content);
          await LoadAsync(content, cancellationToken);
        }
      }
    }

    return contents.AsReadOnly();
  }

  private async Task<IReadOnlyCollection<PokemonSpecies>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_pokeApi.DataDirectory, "data/api/v2/pokemon-species");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    List<PokemonSpecies> speciesList = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, Constants.Encoding, cancellationToken);
      PokemonSpecies? species = ToolsSerializer.Instance.Deserialize<PokemonSpecies>(json);
      if (species is not null)
      {
        speciesList.Add(species);
      }
    }
    return speciesList.AsReadOnly();
  }

  private ContentPayload? Transform(PokemonSpecies species)
  {
    string? displayName = species.GetDisplayName(Constants.Language);

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = species.UniqueName;
    content.Invariant.DisplayName = displayName;

    ContentLocalePayload locale = new()
    {
      UniqueName = species.UniqueName,
      DisplayName = displayName
    };
    content.Locales[Constants.Language] = locale;

    PokedexNumber[] pokedexNumbers = species.PokedexNumbers.Where(x => x.Pokedex.UniqueName == Constants.NationalPokedex).ToArray();
    if (pokedexNumbers.Length == 1)
    {
      content.Invariant.FieldValues[nameof(SpeciesDefinition.Number)] = pokedexNumbers[0].Number.ToString();
    }
    else
    {
      _logger.LogWarning("The Pokémon species '{Species}' must have exactly 1 national Pokédex number (Count: {Count}).", species, pokedexNumbers.Length);
    }

    SpeciesCategory? category = Categorize(species);
    if (category.HasValue)
    {
      content.Invariant.FieldValues[nameof(SpeciesDefinition.Category)] = $"[\"{category}\"]";
    }
    else
    {
      _logger.LogWarning("The Pokémon species '{Species}' cannot be categorized.", species);
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
      _logger.LogWarning("The Pokémon species '{Species}' growth rate '{GrowthRate}' is not valid.", species, species.GrowthRate.UniqueName);
    }

    content.Invariant.FieldValues[nameof(SpeciesDefinition.EggCycles)] = species.EggCycles.ToString();

    if (species.EggGroups.Count < 1 || species.EggGroups.Count > 2)
    {
      _logger.LogWarning("The Pokémon species '{Species}' does not have a valid egg group count '{Count}'.", species, species.EggGroups.Count);
    }
    else
    {
      NamedResource eggGroupValue = species.EggGroups.First();
      EggGroup? eggGroup = ParseEggGroup(eggGroupValue);
      if (eggGroup.HasValue)
      {
        content.Invariant.FieldValues[nameof(SpeciesDefinition.PrimaryEggGroup)] = $"[\"{eggGroup}\"]";
      }
      else
      {
        _logger.LogWarning("The Pokémon species '{Species}' primary egg group '{EggGroup}' is not valid.", species, eggGroupValue.UniqueName);
      }

      if (species.EggGroups.Count == 2)
      {
        eggGroupValue = species.EggGroups.Last();
        eggGroup = ParseEggGroup(eggGroupValue);
        if (eggGroup.HasValue)
        {
          content.Invariant.FieldValues[nameof(SpeciesDefinition.SecondaryEggGroup)] = $"[\"{eggGroup}\"]";
        }
        else
        {
          _logger.LogWarning("The Pokémon species '{Species}' secondary egg group '{EggGroup}' is not valid.", species, eggGroupValue.UniqueName);
        }
      }
    }

    return content;
  }
  private static SpeciesCategory? Categorize(PokemonSpecies species)
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
    return categories.Count > 1 ? null : categories.SingleOrDefault();
  }
  private static EggGroup? ParseEggGroup(NamedResource namedResource) => namedResource.UniqueName switch
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
  private static GrowthRate? ParseGrowthRate(PokemonSpecies species) => species.GrowthRate.UniqueName switch
  {
    "fast" => GrowthRate.Fast,
    "fast-then-very-slow" => GrowthRate.Fluctuating,
    "medium" => GrowthRate.MediumFast,
    "medium-slow" => GrowthRate.MediumSlow,
    "slow" => GrowthRate.Slow,
    "slow-then-very-fast" => GrowthRate.Erratic,
    _ => null,
  };

  private static async Task LoadAsync(ContentPayload content, CancellationToken cancellationToken)
  {
    string path = Path.Combine(OutputDirectory, $"{content.Invariant.UniqueName}.json");
    string json = ToolsSerializer.Instance.Serialize(content);
    await File.WriteAllTextAsync(path, json, Constants.Encoding, cancellationToken);
  }
}
