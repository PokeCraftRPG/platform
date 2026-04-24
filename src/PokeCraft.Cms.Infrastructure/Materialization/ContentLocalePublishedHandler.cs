using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Logging;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EventId = Logitar.EventSourcing.EventId;
using Stream = Logitar.EventSourcing.Stream;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal class ContentLocalePublishedHandler : ContentLocaleEventHandler, IEventHandler<ContentLocalePublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<ContentLocalePublished>, ContentLocalePublishedHandler>();
    services.AddTransient<ICommandHandler<PublishAbilityCommand, Unit>, PublishAbilityCommandHandler>();
    services.AddTransient<ICommandHandler<PublishMoveCommand, Unit>, PublishMoveCommandHandler>();
    services.AddTransient<ICommandHandler<PublishSpeciesCommand, Unit>, PublishSpeciesCommandHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly IEventStore _eventStore;
  private readonly ILoggingService _loggingService;

  public ContentLocalePublishedHandler(
  ICommandBus commandBus,
    IEventStore eventStore,
    KrakenarContext krakenar,
    ILogger<ContentLocalePublishedHandler> logger,
    ILoggingService loggingService) : base(krakenar, logger)
  {
    _commandBus = commandBus;
    _eventStore = eventStore;
    _loggingService = loggingService;
  }

  public async Task HandleAsync(ContentLocalePublished @event, CancellationToken cancellationToken)
  {
    try
    {
      ContentId contentId = new(@event.StreamId);
      if (contentId.RealmId.HasValue)
      {
        Logger.LogWarning("Event 'Id={EventId}' is being ignored because it has a realm ID.", @event.Id);
        return;
      }

      LanguageId? defaultLanguageId = await GetDefaultLanguageIdAsync(@event.Id, @event.LanguageId, cancellationToken);
      if (!defaultLanguageId.HasValue)
      {
        return;
      }

      EntityKind? kind = await GetEntityKindAsync(@event.StreamId, @event.Id, cancellationToken);
      if (!kind.HasValue)
      {
        return;
      }

      FetchOptions options = new()
      {
        ToVersion = @event.Version,
        IsDeleted = false
      };
      Stream? stream = await _eventStore.FetchAsync(@event.StreamId, options, cancellationToken);
      if (stream is null)
      {
        Logger.LogWarning("Event 'Id={EventId}' is being ignored because its stream 'Id={StreamId}' was not found.", @event.Id, @event.StreamId);
        return;
      }

      PublishedContentLocales? published = GetPublishedContentLocales(stream.Events, defaultLanguageId.Value, @event.Id);
      if (published is null)
      {
        return;
      }

      switch (kind.Value)
      {
        case EntityKind.Ability:
          await _commandBus.ExecuteAsync(new PublishAbilityCommand(@event, published.Invariant, published.Locale), cancellationToken);
          break;
        case EntityKind.Move:
          await _commandBus.ExecuteAsync(new PublishMoveCommand(@event, published.Invariant, published.Locale), cancellationToken);
          break;
        case EntityKind.Species:
          await _commandBus.ExecuteAsync(new PublishSpeciesCommand(@event, published.Invariant, published.Locale), cancellationToken);
          break;
        default:
          Logger.LogWarning("Event 'Id={EventId}' is being ignored because the entity kind '{Kind}' is not supported.", @event.Id, kind);
          return;
      }

      Logger.LogInformation("Event 'Id={EventId}' handled successfully.", @event.Id);
    }
    catch (Exception exception)
    {
      _loggingService.Report(exception);
      Logger.LogError(exception, "Event 'Id={EventId}' was not handled successfully.", @event.Id);
    }
  }

  private record PublishedContentLocales(ContentLocale Invariant, ContentLocale Locale);
  private PublishedContentLocales? GetPublishedContentLocales(IEnumerable<Event> events, LanguageId defaultLanguageId, EventId eventId)
  {
    ContentLocale? latestInvariant = null;
    ContentLocale? latestLocale = null;
    ContentLocale? publishedInvariant = null;
    ContentLocale? publishedLocale = null;
    foreach (Event change in events)
    {
      if (change.Data is ContentCreated created)
      {
        latestInvariant = created.Invariant;
      }
      else if (change.Data is ContentLocaleChanged changed)
      {
        if (changed.LanguageId is null)
        {
          latestInvariant = changed.Locale;
        }
        else if (changed.LanguageId == defaultLanguageId)
        {
          latestLocale = changed.Locale;
        }
      }
      else if (change.Data is ContentLocalePublished published)
      {
        if (published.LanguageId is null)
        {
          publishedInvariant = latestInvariant;
        }
        else if (published.LanguageId == defaultLanguageId)
        {
          publishedLocale = latestLocale;
        }
      }
      else if (change.Data is ContentLocaleRemoved removed)
      {
        if (removed.LanguageId == defaultLanguageId)
        {
          latestLocale = null;
          publishedLocale = null;
        }
      }
      else if (change.Data is ContentLocaleUnpublished unpublished)
      {
        if (unpublished.LanguageId is null)
        {
          publishedInvariant = null;
        }
        else if (unpublished.LanguageId == defaultLanguageId)
        {
          publishedLocale = null;
        }
      }
    }

    if (publishedInvariant is null)
    {
      Logger.LogWarning("Event 'Id={EventId}' is being ignored because the invariant is not published.", eventId);
      return null;
    }
    else if (publishedLocale is null)
    {
      Logger.LogWarning("Event 'Id={EventId}' is being ignored because the locale is not published.", eventId);
      return null;
    }

    return new PublishedContentLocales(publishedInvariant, publishedLocale);
  }
}
