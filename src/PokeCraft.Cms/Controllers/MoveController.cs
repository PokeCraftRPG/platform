using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Moves;
using PokeCraft.Cms.Core.Moves.Models;
using PokeCraft.Cms.Models.Move;

namespace PokeCraft.Cms.Controllers;

[ApiController]
[Route("api/moves")]
public class MoveController : ControllerBase
{
  private readonly IMoveService _moveService;

  public MoveController(IMoveService moveService)
  {
    _moveService = moveService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Move>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Move? move = await _moveService.ReadAsync(id, key: null, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpGet("key:{key}")]
  public async Task<ActionResult<Move>> ReadAsync(string key, CancellationToken cancellationToken)
  {
    Move? move = await _moveService.ReadAsync(id: null, key, cancellationToken);
    return move is null ? NotFound() : Ok(move);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Move>>> SearchAsync(SearchMovesParameters parameters, CancellationToken cancellationToken)
  {
    SearchMovesPayload payload = parameters.ToPayload();
    SearchResults<Move> results = await _moveService.SearchAsync(payload, cancellationToken);
    return Ok(results);
  }
}
