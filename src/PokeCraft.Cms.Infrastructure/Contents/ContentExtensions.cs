using Krakenar.Core.Contents;
using Krakenar.Core.Fields;

namespace PokeCraft.Cms.Infrastructure.Contents;

internal static class ContentExtensions
{
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
}
