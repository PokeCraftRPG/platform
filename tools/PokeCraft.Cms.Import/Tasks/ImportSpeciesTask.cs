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

  private readonly PokeApiSettings _pokeApi;

  public ImportSpeciesTask(PokeApiSettings pokeApi)
  {
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

  private static ContentPayload? Transform(PokemonSpecies species)
  {
    PokedexNumber[] pokedexNumbers = species.PokedexNumbers.Where(x => x.Pokedex.UniqueName == Constants.NationalPokedex).ToArray();
    SpeciesCategory? category = Categorize(species);
    GrowthRate? growthRate = ParseGrowthRate(species);
    if (pokedexNumbers.Length != 1 || !category.HasValue || !growthRate.HasValue || species.EggGroups.Count < 1 || species.EggGroups.Count > 2)
    {
      return null;
    }

    EggGroup? primaryEggGroup = ParseEggGroup(species.EggGroups.First());
    EggGroup? secondaryEggGroup = null;
    if (!primaryEggGroup.HasValue)
    {
      return null;
    }
    else if (species.EggGroups.Count == 2)
    {
      secondaryEggGroup = ParseEggGroup(species.EggGroups.Last());
      if (!secondaryEggGroup.HasValue)
      {
        return null;
      }
    }

    string? displayName = species.GetDisplayName(Constants.Language);

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = species.UniqueName;
    content.Invariant.DisplayName = displayName;

    content.Invariant.FieldValues[nameof(SpeciesDefinition.Number)] = pokedexNumbers[0].Number.ToString();
    content.Invariant.FieldValues[nameof(SpeciesDefinition.Category)] = $"[\"{category}\"]";

    content.Invariant.FieldValues[nameof(SpeciesDefinition.BaseFriendship)] = species.BaseFriendship.ToString();
    content.Invariant.FieldValues[nameof(SpeciesDefinition.CatchRate)] = species.CatchRate.ToString();
    content.Invariant.FieldValues[nameof(SpeciesDefinition.GrowthRate)] = $"[\"{growthRate}\"]";

    content.Invariant.FieldValues[nameof(SpeciesDefinition.EggCycles)] = species.EggCycles.ToString();
    content.Invariant.FieldValues[nameof(SpeciesDefinition.PrimaryEggGroup)] = $"[\"{primaryEggGroup}\"]";
    if (secondaryEggGroup.HasValue)
    {
      content.Invariant.FieldValues[nameof(SpeciesDefinition.SecondaryEggGroup)] = $"[\"{secondaryEggGroup}\"]";
    }

    ContentLocalePayload locale = new()
    {
      UniqueName = species.UniqueName,
      DisplayName = displayName
    };
    content.Locales[Constants.Language] = locale;

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
