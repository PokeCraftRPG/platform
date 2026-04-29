using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Seeding.Tasks;

internal class SeedContentsTask : SeedingTask
{
  public override string? Description => "Seeds the Contents into Krakenar.";

  public Guid ContentTypeId { get; }
  public string DefaultLanguage { get; }
  public string Directory { get; }

  public SeedContentsTask(Guid contentTypeId, string defaultLanguage, string directory)
  {
    ContentTypeId = contentTypeId;
    DefaultLanguage = defaultLanguage;
    Directory = directory;
  }
}

internal class SeedContentsTaskHandler : ICommandHandler<SeedContentsTask, Unit>
{
  private readonly IContentService _contentService;
  private readonly KrakenarContext _krakenar;
  private readonly ILogger<SeedContentsTaskHandler> _logger;

  public SeedContentsTaskHandler(IContentService contentService, KrakenarContext krakenar, ILogger<SeedContentsTaskHandler> logger)
  {
    _contentService = contentService;
    _krakenar = krakenar;
    _logger = logger;
  }

  public async Task<Unit> HandleAsync(SeedContentsTask task, CancellationToken cancellationToken)
  {
    var contentType = await _krakenar.ContentTypes
      .Where(x => x.Id == task.ContentTypeId)
      .Select(x => new { x.UniqueName, x.IsInvariant })
      .SingleOrDefaultAsync(cancellationToken)
      ?? throw new InvalidOperationException($"The content type 'Id={task.ContentTypeId}' was not found.");
    string? defaultLanguage = contentType.IsInvariant ? null : task.DefaultLanguage;

    HashSet<Guid> existingIds = await _krakenar.Contents
      .Where(x => x.ContentTypeUid == task.ContentTypeId)
      .Select(x => x.Id)
      .ToHashSetAsync(cancellationToken);

    IReadOnlyCollection<ContentPayload> payloads = await ExtractAsync(task.Directory, cancellationToken);
    while (payloads.Count > 0)
    {
      IReadOnlyCollection<Failure<ContentPayload>> failures = await LoadAsync(payloads, existingIds, contentType.UniqueName, defaultLanguage, cancellationToken);
      if (failures.Count >= payloads.Count)
      {
        throw failures.First().Exception;
      }
      payloads = failures.Select(x => x.Value).ToList().AsReadOnly();
    }

    return Unit.Value;
  }

  private static async Task<IReadOnlyCollection<ContentPayload>> ExtractAsync(string directory, CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(directory);
    string[] paths = Directory.GetFiles(directory, "*.json");
    List<ContentPayload> payloads = new(capacity: paths.Length);

    foreach (string path in paths)
    {
      string json = await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken);
      ContentPayload? payload = ToolsSerializer.Instance.Deserialize<ContentPayload>(json);
      if (payload is not null)
      {
        payloads.Add(payload);
      }
    }

    return payloads.AsReadOnly();
  }

  private async Task<IReadOnlyCollection<Failure<ContentPayload>>> LoadAsync(
    IReadOnlyCollection<ContentPayload> payloads,
    HashSet<Guid> existingIds,
    string contentType,
    string? defaultLanguage,
    CancellationToken cancellationToken)
  {
    List<Failure<ContentPayload>> failures = new(capacity: payloads.Count);

    foreach (ContentPayload payload in payloads)
    {
      try
      {
        Guid contentId = payload.Id;
        ContentLocalePayload invariant = payload.Invariant;

        Content content;
        bool created = false;
        if (existingIds.Contains(contentId))
        {
          SaveContentLocalePayload savePayload = new()
          {
            UniqueName = invariant.UniqueName,
            DisplayName = invariant.DisplayName,
            Description = invariant.Description
          };
          savePayload.FieldValues.AddRange(invariant.FieldValues.Select(field => new FieldValuePayload(field.Key, field.Value)));
          content = await _contentService.SaveLocaleAsync(contentId, savePayload, language: null, cancellationToken)
            ?? throw new InvalidOperationException($"SaveInvariant: the saved content 'Id={contentId}' should not be null.");
        }
        else
        {
          CreateContentPayload createPayload = new()
          {
            Id = contentId,
            ContentType = contentType,
            Language = defaultLanguage,
            UniqueName = invariant.UniqueName,
            DisplayName = invariant.DisplayName,
            Description = invariant.Description
          };
          createPayload.FieldValues.AddRange(invariant.FieldValues.Select(field => new FieldValuePayload(field.Key, field.Value)));
          content = await _contentService.CreateAsync(createPayload, cancellationToken);
          existingIds.Add(content.Id);
          created = true;
        }

        if (invariant.IsPublished)
        {
          content = await _contentService.PublishAsync(content.Id, language: null, cancellationToken)
            ?? throw new InvalidOperationException($"PublishInvariant: the content 'Id={contentId}' should not be null.");
        }
        else
        {
          content = await _contentService.UnpublishAsync(content.Id, language: null, cancellationToken)
            ?? throw new InvalidOperationException($"UnpublishInvariant: the content 'Id={contentId}' should not be null.");
        }

        foreach (KeyValuePair<string, ContentLocalePayload> pair in payload.Locales)
        {
          string language = pair.Key;
          ContentLocalePayload locale = pair.Value;

          SaveContentLocalePayload savePayload = new()
          {
            UniqueName = locale.UniqueName,
            DisplayName = locale.DisplayName,
            Description = locale.Description
          };
          savePayload.FieldValues.AddRange(locale.FieldValues.Select(field => new FieldValuePayload(field.Key, field.Value)));
          content = await _contentService.SaveLocaleAsync(content.Id, savePayload, language, cancellationToken)
            ?? throw new InvalidOperationException($"SaveLocale({language}): the content 'Id={contentId}' should not be null.");

          if (locale.IsPublished)
          {
            content = await _contentService.PublishAsync(content.Id, language, cancellationToken)
              ?? throw new InvalidOperationException($"PublishLocale({language}): the content 'Id={contentId}' should not be null.");
          }
          else
          {
            content = await _contentService.UnpublishAsync(content.Id, language, cancellationToken)
              ?? throw new InvalidOperationException($"UnpublishLocale({language}): the content 'Id={contentId}' should not be null.");
          }
        }

        _logger.LogInformation("The {ContentType} content '{Content} (Id={ContentId})' was {Action}.",
          content.ContentType.UniqueName,
          content.Invariant.DisplayName ?? content.Invariant.UniqueName,
          content.Id,
          created ? "created" : "replaced");
      }
      catch (Exception exception)
      {
        failures.Add(new Failure<ContentPayload>(payload, exception));
      }
    }

    return failures.AsReadOnly();
  }
}
