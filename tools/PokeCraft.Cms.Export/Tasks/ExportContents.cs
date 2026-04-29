using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Localization;
using Krakenar.EntityFrameworkCore.Relational;
using Logitar.CQRS;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Cms.Tools;
using PokeCraft.Cms.Tools.Models;

namespace PokeCraft.Cms.Export.Tasks;

internal class ExportContentsTask : ExportTask
{
  public override string? Description => "Exports the Contents of a specified type.";

  public Guid ContentTypeId { get; }
  public string Directory { get; }

  public ExportContentsTask(Guid contentTypeId, string directory)
  {
    ContentTypeId = contentTypeId;
    Directory = directory;
  }
}

internal class ExportContentsTaskHandler : ICommandHandler<ExportContentsTask, Unit>
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly KrakenarContext _krakenar;
  private readonly ILogger<ExportContentsTaskHandler> _logger;

  public ExportContentsTaskHandler(
    IContentRepository contentRepository,
    IContentTypeRepository contentTypeRepository,
    KrakenarContext krakenar,
    ILogger<ExportContentsTaskHandler> logger)
  {
    _contentRepository = contentRepository;
    _contentTypeRepository = contentTypeRepository;
    _krakenar = krakenar;
    _logger = logger;
  }

  public async Task<Unit> HandleAsync(ExportContentsTask task, CancellationToken cancellationToken)
  {
    Directory.CreateDirectory(task.Directory);

    ContentTypeId contentTypeId = new(task.ContentTypeId);
    ContentType contentType = await _contentTypeRepository.LoadAsync(contentTypeId, cancellationToken)
      ?? throw new InvalidOperationException($"The content type 'Id={contentTypeId}' was not found.");

    Dictionary<LanguageId, string> languages = (await _krakenar.Languages.AsNoTracking()
      .Where(x => x.RealmId == null)
      .Select(x => new { x.StreamId, x.Code })
      .ToArrayAsync(cancellationToken)).ToDictionary(x => new LanguageId(x.StreamId), x => x.Code.Trim().ToLowerInvariant());

    string[] streamIds = await _krakenar.Contents.AsNoTracking()
      .Where(x => x.RealmId == null && x.ContentTypeUid == task.ContentTypeId)
      .Select(x => x.StreamId)
      .ToArrayAsync(cancellationToken);
    IEnumerable<ContentId> contentIds = streamIds.Select(streamId => new ContentId(streamId));
    IReadOnlyCollection<Content> contents = await _contentRepository.LoadAsync(contentIds, cancellationToken);

    foreach (Content content in contents)
    {
      if (content.IsDeleted)
      {
        continue;
      }

      ContentPayload payload = new()
      {
        Id = content.EntityId
      };

      payload.Invariant.IsPublished = content.GetInvariantStatus() == ContentStatus.Latest;
      Populate(payload.Invariant, contentType, content.Invariant);

      foreach (KeyValuePair<LanguageId, ContentLocale> pair in content.Locales)
      {
        LanguageId languageId = pair.Key;
        ContentLocale locale = pair.Value;

        ContentLocalePayload localePayload = new()
        {
          IsPublished = content.GetLocaleStatus(languageId) == ContentStatus.Latest
        };
        Populate(localePayload, contentType, locale);
        payload.Locales[languages[languageId]] = localePayload;
      }

      string path = Path.Combine(task.Directory, $"{payload.Invariant.UniqueName}.json");
      string json = ToolsSerializer.Instance.Serialize(payload);
      await File.WriteAllTextAsync(path, json, Encoding.UTF8, cancellationToken);

      _logger.LogInformation("The {ContentType} content '{Content} (Id={ContentId})' was exported.",
        contentType.UniqueName,
        payload.Invariant.DisplayName ?? payload.Invariant.UniqueName,
        content.EntityId);
    }

    return Unit.Value;
  }

  private static void Populate(ContentLocalePayload payload, ContentType contentType, ContentLocale locale)
  {
    payload.UniqueName = locale.UniqueName.Value;
    payload.DisplayName = locale.DisplayName?.Value;
    payload.Description = locale.Description?.Value;

    foreach (KeyValuePair<Guid, FieldValue> field in locale.FieldValues)
    {
      string key = contentType.FindField(field.Key).UniqueName.Value;
      payload.FieldValues[key] = field.Value.Value;
    }
  }
}
