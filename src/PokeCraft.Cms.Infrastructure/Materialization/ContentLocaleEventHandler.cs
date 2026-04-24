using Krakenar.Core.Localization;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EventId = Logitar.EventSourcing.EventId;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal abstract class ContentLocaleEventHandler
{
  protected KrakenarContext Krakenar { get; set; }
  protected ILogger Logger { get; set; }

  protected ContentLocaleEventHandler(KrakenarContext krakenar, ILogger logger)
  {
    Krakenar = krakenar;
    Logger = logger;
  }

  protected async Task<LanguageId?> GetDefaultLanguageIdAsync(EventId eventId, LanguageId? eventLanguageId, CancellationToken cancellationToken)
  {
    string? defaultLanguageStreamId = await Krakenar.Languages.AsNoTracking()
      .Where(x => x.RealmId == null && x.IsDefault)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);
    if (string.IsNullOrWhiteSpace(defaultLanguageStreamId))
    {
      Logger.LogWarning("Event 'Id={EventId}' is being ignored because the defaut language was not found.", @eventId);
      return null;
    }

    LanguageId defaultLanguageId = new(defaultLanguageStreamId);
    if (eventLanguageId.HasValue && eventLanguageId.Value != defaultLanguageId)
    {
      Logger.LogWarning("Event 'Id={EventId}' is being ignored because the language 'Id={LanguageId}' is not tracked.", eventId, eventLanguageId.Value);
      return null;
    }
    return defaultLanguageId;
  }

  protected async Task<EntityKind?> GetEntityKindAsync(StreamId streamId, EventId eventId, CancellationToken cancellationToken)
  {
    string? contentType = await Krakenar.Contents.AsNoTracking()
      .Include(x => x.ContentType)
      .Where(x => x.StreamId == streamId.Value)
      .Select(x => x.ContentType!.UniqueName)
      .SingleOrDefaultAsync(cancellationToken);

    if (string.IsNullOrWhiteSpace(contentType) || !Enum.TryParse(contentType.Trim(), ignoreCase: true, out EntityKind kind) || !Enum.IsDefined(kind))
    {
      Logger.LogWarning("Event 'Id={EventId}' is being ignored because its content type '{ContentType}' is not tracked.", eventId, contentType);
      return null;
    }

    return kind;
  }
}
