using Krakenar.Core.Contents;
using Krakenar.Core.Fields;

namespace PokeCraft.Cms.Infrastructure.Contents;

internal static class ContentExtensions
{
  public static bool GetBoolean(this ContentLocale locale, Guid id, bool defaultValue = false)
  {
    return locale.TryGetBoolean(id) ?? defaultValue;
  }
  public static bool? TryGetBoolean(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? field) && bool.TryParse(field.Value, out bool boolean)
      ? boolean
      : null;
  }

  public static IReadOnlyCollection<Guid> GetRelatedContent(this ContentLocale locale, Guid id, IReadOnlyCollection<Guid>? defaultValue = null)
  {
    return locale.TryGetRelatedContent(id) ?? defaultValue ?? [];
  }
  public static IReadOnlyCollection<Guid>? TryGetRelatedContent(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? field)
      ? JsonSerializer.Deserialize<IReadOnlyCollection<Guid>>(field.Value)
      : null;
  }

  public static IReadOnlyCollection<string> GetSelect(this ContentLocale locale, Guid id, IReadOnlyCollection<string>? defaultValue = null)
  {
    return locale.TryGetSelect(id) ?? defaultValue ?? [];
  }
  public static IReadOnlyCollection<string>? TryGetSelect(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? field)
      ? JsonSerializer.Deserialize<IReadOnlyCollection<string>>(field.Value)
      : null;
  }

  public static double GetNumber(this ContentLocale locale, Guid id, double defaultValue = 0.0)
  {
    return locale.TryGetNumber(id) ?? defaultValue;
  }
  public static double? TryGetNumber(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? field) && double.TryParse(field.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double number)
      ? number
      : null;
  }

  public static string GetString(this ContentLocale locale, Guid id, string defaultValue = "")
  {
    return locale.TryGetString(id) ?? defaultValue;
  }
  public static string? TryGetString(this ContentLocale locale, Guid id)
  {
    return locale.FieldValues.TryGetValue(id, out FieldValue? field) ? field.Value : null;
  }
}
