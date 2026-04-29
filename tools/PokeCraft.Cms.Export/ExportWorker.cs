using Logitar.CQRS;
using PokeCraft.Cms.Export.Tasks;
using PokeCraft.Cms.Infrastructure.Contents;

namespace PokeCraft.Cms.Export;

internal class ExportWorker : BackgroundService
{
  private const string GenericErrorMessage = "An unhanded exception occurred.";

  private readonly IHostApplicationLifetime _hostApplicationLifetime;
  private readonly ILogger<ExportWorker>? _logger;
  private readonly IServiceProvider _serviceProvider;

  private ICommandBus? _commandBus = null;
  private ICommandBus CommandBus => _commandBus ?? throw new InvalidOperationException("The command bus has not been initialized.");

  private LogLevel _result = LogLevel.Information; // NOTE(fpion): "Information" means success.

  public ExportWorker(IServiceProvider serviceProvider)
  {
    _hostApplicationLifetime = serviceProvider.GetRequiredService<IHostApplicationLifetime>();
    _logger = serviceProvider.GetService<ILogger<ExportWorker>>();
    _serviceProvider = serviceProvider;
  }

  protected override async Task ExecuteAsync(CancellationToken cancellationToken)
  {
    Stopwatch chrono = Stopwatch.StartNew();
    _logger?.LogInformation("Worker executing at {Timestamp}.", DateTimeOffset.Now);

    using IServiceScope scope = _serviceProvider.CreateScope();
    _commandBus = scope.ServiceProvider.GetService<ICommandBus>();

    try
    {
      // NOTE(fpion): the order of these tasks matter.
      await ExecuteAsync(new ExportContentsTask(AbilityDefinition.ContentTypeId, "data/attributes"), cancellationToken);
      await ExecuteAsync(new ExportContentsTask(MoveDefinition.ContentTypeId, "data/moves"), cancellationToken);
      await ExecuteAsync(new ExportContentsTask(SpeciesDefinition.ContentTypeId, "data/species"), cancellationToken);
      await ExecuteAsync(new ExportContentsTask(VarietyDefinition.ContentTypeId, "data/varieties"), cancellationToken);
      await ExecuteAsync(new ExportContentsTask(FormDefinition.ContentTypeId, "data/forms"), cancellationToken);
    }
    catch (Exception exception)
    {
      _logger?.LogError(exception, GenericErrorMessage);
      _result = LogLevel.Error;

      Environment.ExitCode = exception.HResult;
    }
    finally
    {
      chrono.Stop();

      if (_logger is not null)
      {
        long seconds = chrono.ElapsedMilliseconds / 1000;
        string secondText = seconds <= 1 ? "second" : "seconds";
        switch (_result)
        {
          case LogLevel.Error:
            _logger.LogError("Export failed after {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
            break;
          case LogLevel.Warning:
            _logger.LogWarning("Export completed with warnings in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
            break;
          default:
            _logger.LogInformation("Export succeeded in {Elapsed}ms ({Seconds} {SecondText}).", chrono.ElapsedMilliseconds, seconds, secondText);
            break;
        }
      }

      _hostApplicationLifetime.StopApplication();
    }
  }

  private async Task ExecuteAsync(ExportTask task, CancellationToken cancellationToken)
  {
    await ExecuteAsync(task, continueOnError: false, cancellationToken);
  }
  private async Task ExecuteAsync(ExportTask task, bool continueOnError, CancellationToken cancellationToken)
  {
    bool hasFailed = false;
    try
    {
      await CommandBus.ExecuteAsync(task, cancellationToken);
    }
    catch (Exception exception)
    {
      if (continueOnError)
      {
        _logger?.LogWarning(exception, GenericErrorMessage);
        hasFailed = true;
      }
      else
      {
        throw;
      }
    }
    finally
    {
      task.Complete();

      LogLevel result = LogLevel.Information;
      if (hasFailed)
      {
        _result = LogLevel.Warning;
        result = LogLevel.Warning;
      }

      if (_logger is not null)
      {
        int milliseconds = task.Duration?.Milliseconds ?? 0;
        int seconds = milliseconds / 1000;
        string secondText = seconds <= 1 ? "second" : "seconds";
        _logger.Log(result, "Task '{Name}' succeeded in {Elapsed}ms ({Seconds} {SecondText}).", task.Name, milliseconds, seconds, secondText);
      }
    }
  }
}
