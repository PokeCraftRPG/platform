using PokeCraft.Cms.Core;
using PokeCraft.Cms.Core.Forms;
using PokeCraft.Cms.Import.Models;
using PokeCraft.Cms.Import.Settings;
using PokeCraft.Cms.Infrastructure.Contents;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Import.Tasks;

internal class ImportFormsTask
{
  private const string OutputDirectory = "data/forms";

  private readonly ILogger<ImportFormsTask> _logger;
  private readonly PokeApiSettings _pokeApi;

  public ImportFormsTask(ILogger<ImportFormsTask> logger, PokeApiSettings pokeApi)
  {
    _logger = logger;
    _pokeApi = pokeApi;
  }

  public async Task<IReadOnlyDictionary<string, Imported<Form>>> ExecuteAsync(
    IReadOnlyDictionary<string, Imported<Ability>> abilities,
    IReadOnlyDictionary<string, Imported<PokemonSpecies>> species,
    IReadOnlyDictionary<string, Imported<Variety>> varieties,
    CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(OutputDirectory);

    IReadOnlyCollection<Form> forms = await ExtractAsync(cancellationToken);
    Dictionary<string, Imported<Form>> contents = new(capacity: forms.Count);

    foreach (Form form in forms)
    {
      if (!string.IsNullOrWhiteSpace(form.UniqueName))
      {
        ContentPayload? content = Transform(form, abilities, species, varieties);
        if (content is not null)
        {
          contents[content.Invariant.UniqueName] = new Imported<Form>(form, content);
          await LoadAsync(content, cancellationToken);
        }
      }
    }

    return contents.AsReadOnly();
  }

  private async Task<IReadOnlyCollection<Form>> ExtractAsync(CancellationToken cancellationToken)
  {
    string directory = Path.Combine(_pokeApi.DataDirectory, "data/api/v2/pokemon-form");
    string[] paths = Directory.GetFiles(directory, "*.json", SearchOption.AllDirectories);
    List<Form> forms = new(capacity: paths.Length);
    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, Constants.Encoding, cancellationToken);
      Form? form = ToolsSerializer.Instance.Deserialize<Form>(json);
      if (form is not null)
      {
        forms.Add(form);
      }
    }
    return forms.AsReadOnly();
  }

  private ContentPayload? Transform(
    Form form,
    IReadOnlyDictionary<string, Imported<Ability>> abilities,
    IReadOnlyDictionary<string, Imported<PokemonSpecies>> importedSpecies,
    IReadOnlyDictionary<string, Imported<Variety>> varieties)
  {
    if (!varieties.TryGetValue(form.Variety.UniqueName, out Imported<Variety>? variety))
    {
      _logger.LogWarning("The Pokémon form '{Form}' variety '{Variety}' was not found.", form, form.Variety.UniqueName);
    }

    Imported<PokemonSpecies>? species = null;
    if (variety is not null && !importedSpecies.TryGetValue(variety.Entity.Species.UniqueName, out species))
    {
      _logger.LogWarning("The Pokémon variety '{Variety}' species '{Species}' was not found.", variety.Entity, variety.Entity.Species.UniqueName);
    }

    string? displayName = form.GetDisplayName(Constants.Language) ?? species?.Entity.GetDisplayName(Constants.Language);

    ContentPayload content = new()
    {
      Id = Guid.NewGuid()
    };
    content.Invariant.UniqueName = form.UniqueName;
    content.Invariant.DisplayName = displayName;

    ContentLocalePayload locale = new()
    {
      UniqueName = form.UniqueName,
      DisplayName = displayName
    };
    content.Locales[Constants.Language] = locale;

    if (variety is not null)
    {
      content.Invariant.FieldValues[nameof(FormDefinition.Variety)] = $"[\"{variety.Content.Id}\"]";

      content.Invariant.FieldValues[nameof(FormDefinition.Height)] = variety.Entity.Height.ToString();
      content.Invariant.FieldValues[nameof(FormDefinition.Weight)] = variety.Entity.Weight.ToString();

      if (variety.Entity.ExperienceYield.HasValue)
      {
        content.Invariant.FieldValues[nameof(FormDefinition.YieldExperience)] = variety.Entity.ExperienceYield.Value.ToString();
      }

      foreach (SlottedAbility slottedAbility in variety.Entity.Abilities)
      {
        if (!abilities.TryGetValue(slottedAbility.Ability.UniqueName, out Imported<Ability>? ability))
        {
          _logger.LogWarning("The Pokémon variety '{Variety}' ability '{Ability}' was not found.", variety.Entity, slottedAbility.Ability.UniqueName);
          continue;
        }

        switch (slottedAbility.Slot)
        {
          case 1:
            if (slottedAbility.IsHidden)
            {
              _logger.LogWarning("The Pokémon variety '{Variety}' ability slot '{Slot}' (IsHidden: {IsHidden}) is not valid.", variety.Entity, slottedAbility.Slot, slottedAbility.IsHidden);
              continue;
            }
            content.Invariant.FieldValues[nameof(FormDefinition.PrimaryAbility)] = $"[\"{ability.Content.Id}\"]";
            break;
          case 2:
            if (slottedAbility.IsHidden)
            {
              _logger.LogWarning("The Pokémon variety '{Variety}' ability slot '{Slot}' (IsHidden: {IsHidden}) is not valid.", variety.Entity, slottedAbility.Slot, slottedAbility.IsHidden);
              continue;
            }
            content.Invariant.FieldValues[nameof(FormDefinition.SecondaryAbility)] = $"[\"{ability.Content.Id}\"]";
            break;
          case 3:
            if (!slottedAbility.IsHidden)
            {
              _logger.LogWarning("The Pokémon variety '{Variety}' ability slot '{Slot}' (IsHidden: {IsHidden}) is not valid.", variety.Entity, slottedAbility.Slot, slottedAbility.IsHidden);
              continue;
            }
            content.Invariant.FieldValues[nameof(FormDefinition.HiddenAbility)] = $"[\"{ability.Content.Id}\"]";
            break;
          default:
            break;
        }
      }

      foreach (PokemonStatistic statistic in variety.Entity.Statistics)
      {
        switch (statistic.Statistic.UniqueName)
        {
          case "attack":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseAttack)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldAttack)] = statistic.Yield.ToString();
            break;
          case "defense":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseDefense)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldDefense)] = statistic.Yield.ToString();
            break;
          case "hp":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseHP)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldHP)] = statistic.Yield.ToString();
            break;
          case "special-attack":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseSpecialAttack)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldSpecialAttack)] = statistic.Yield.ToString();
            break;
          case "special-defense":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseSpecialDefense)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldSpecialDefense)] = statistic.Yield.ToString();
            break;
          case "speed":
            content.Invariant.FieldValues[nameof(FormDefinition.BaseSpeed)] = statistic.Base.ToString();
            content.Invariant.FieldValues[nameof(FormDefinition.YieldSpeed)] = statistic.Yield.ToString();
            break;
          default:
            _logger.LogWarning("The Pokémon variety '{Variety}' statistic '{Statistic}' is not valid.", variety, statistic.Statistic);
            break;
        }
      }

      if (species is not null)
      {
        content.Invariant.FieldValues[nameof(FormDefinition.HasGenderDifferences)] = species.Entity.HasGenderDifferences.ToString();
      }
    }
    content.Invariant.FieldValues[nameof(FormDefinition.Kind)] = $"[\"{GetFormKind(form)}\"]";

    foreach (FormType formType in form.Types)
    {
      if (!Enum.TryParse(formType.Type.UniqueName, ignoreCase: true, out PokemonType type) || !Enum.IsDefined(type))
      {
        _logger.LogWarning("The Pokémon form '{Form}' type '{Type}' is not valid.", form, formType.Type.UniqueName);
        continue;
      }

      switch (formType.Slot)
      {
        case 1:
          content.Invariant.FieldValues[nameof(FormDefinition.PrimaryType)] = $"[\"{type}\"]";
          break;
        case 2:
          content.Invariant.FieldValues[nameof(FormDefinition.SecondaryType)] = $"[\"{type}\"]";
          break;
        default:
          _logger.LogWarning("The Pokémon form '{Form}' type slot '{Slot}' is not valid.", form, formType.Slot);
          break;
      }
    }

    return content;
  }
  private static FormKind GetFormKind(Form form)
  {
    if (form.IsMega)
    {
      return FormKind.Mega;
    }
    else if (form.IsBattleOnly)
    {
      return FormKind.BattleOnly;
    }
    else if (form.IsDefault)
    {
      return FormKind.Default;
    }
    else
    {
      return FormKind.Alternative;
    }
  }

  private static async Task LoadAsync(ContentPayload content, CancellationToken cancellationToken)
  {
    string path = Path.Combine(OutputDirectory, $"{content.Invariant.UniqueName}.json");
    string json = ToolsSerializer.Instance.Serialize(content);
    await File.WriteAllTextAsync(path, json, Constants.Encoding, cancellationToken);
  }
}
