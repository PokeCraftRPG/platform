using Krakenar.Contracts.Users;
using Krakenar.Core;
using Krakenar.Core.Users;
using Logitar.CQRS;
using Logitar.EventSourcing;
using PokeCraft.Cms.Seeding.Models;
using PokeCraft.Cms.Tools;
using User = Krakenar.Contracts.Users.User;

namespace PokeCraft.Cms.Seeding.Tasks;

internal class SeedUsersTask : SeedingTask
{
  public override string? Description => "Seeds the Users into Krakenar.";
}

internal class SeedUsersTaskHandler : ICommandHandler<SeedUsersTask, Unit>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ILogger<SeedUsersTaskHandler> _logger;
  private readonly IUserService _userService;

  public SeedUsersTaskHandler(IApplicationContext applicationContext, ILogger<SeedUsersTaskHandler> logger, IUserService userService)
  {
    _applicationContext = applicationContext;
    _logger = logger;
    _userService = userService;
  }

  public async Task<Unit> HandleAsync(SeedUsersTask _, CancellationToken cancellationToken)
  {
    string json = await File.ReadAllTextAsync("data/users.json", Encoding.UTF8, cancellationToken);
    IEnumerable<UserPayload>? payloads = ToolsSerializer.Instance.Deserialize<IEnumerable<UserPayload>>(json);
    if (payloads is not null)
    {
      foreach (UserPayload payload in payloads)
      {
        if (!payload.Id.HasValue)
        {
          ActorId? actorId = _applicationContext.ActorId;
          if (!actorId.HasValue)
          {
            _logger.LogWarning("The user '{UniqueName}' is being ignored, because it does not have an ID, and no actor has been specified.", payload.UniqueName);
            continue;
          }

          UserId userId = new(actorId.Value.Value);
          payload.Id = userId.EntityId;
        }

        CreateOrReplaceUserResult result = await _userService.CreateOrReplaceAsync(payload, payload.Id, version: null, cancellationToken);
        User user = result.User ?? throw new InvalidOperationException($"'UserService.CreateOrReplaceAsync' returned null for user 'Id={payload.Id}'.");
        _logger.LogInformation("The user '{User}' was {Action}.", user.FullName ?? user.UniqueName, result.Created ? "created" : "replaced");
      }
    }

    return Unit.Value;
  }
}
