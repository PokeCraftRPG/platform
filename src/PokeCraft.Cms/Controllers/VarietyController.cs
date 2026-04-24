using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Varieties;
using PokeCraft.Cms.Core.Varieties.Models;
using PokeCraft.Cms.Models.Variety;

namespace PokeCraft.Cms.Controllers;

[ApiController]
[Route("api/varieties")]
public class VarietyController : ControllerBase
{
  private readonly IVarietyService _varietyService;

  public VarietyController(IVarietyService varietyService)
  {
    _varietyService = varietyService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Variety>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Variety? variety = await _varietyService.ReadAsync(id, key: null, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet("key:{key}")]
  public async Task<ActionResult<Variety>> ReadAsync(string key, CancellationToken cancellationToken)
  {
    Variety? variety = await _varietyService.ReadAsync(id: null, key, cancellationToken);
    return variety is null ? NotFound() : Ok(variety);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Variety>>> SearchAsync(SearchVarietiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchVarietiesPayload payload = parameters.ToPayload();
    SearchResults<Variety> results = await _varietyService.SearchAsync(payload, cancellationToken);
    return Ok(results);
  }
}
