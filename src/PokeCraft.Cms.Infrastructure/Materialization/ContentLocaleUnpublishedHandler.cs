using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Localization;
using Krakenar.Core.Logging;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.CQRS;
using Logitar.EventSourcing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PokeCraft.Cms.Infrastructure.Materialization;

internal class ContentLocaleUnpublishedHandler : ContentLocaleEventHandler, IEventHandler<ContentLocaleUnpublished>
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IEventHandler<ContentLocaleUnpublished>, ContentLocaleUnpublishedHandler>();
    services.AddTransient<ICommandHandler<UnpublishAbilityCommand, Unit>, UnpublishAbilityCommandHandler>();
    services.AddTransient<ICommandHandler<UnpublishMoveCommand, Unit>, UnpublishMoveCommandHandler>();
  }

  private readonly ICommandBus _commandBus;
  private readonly ILoggingService _loggingService;

  public ContentLocaleUnpublishedHandler(
  ICommandBus commandBus,
    KrakenarContext krakenar,
    ILogger<ContentLocaleUnpublishedHandler> logger,
    ILoggingService loggingService) : base(krakenar, logger)
  {
    _commandBus = commandBus;
    _loggingService = loggingService;
  }

  public async Task HandleAsync(ContentLocaleUnpublished @event, CancellationToken cancellationToken)
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

      switch (kind.Value)
      {
        case EntityKind.Ability:
          await _commandBus.ExecuteAsync(new UnpublishAbilityCommand(@event), cancellationToken);
          break;
        case EntityKind.Move:
          await _commandBus.ExecuteAsync(new UnpublishMoveCommand(@event), cancellationToken);
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
}
