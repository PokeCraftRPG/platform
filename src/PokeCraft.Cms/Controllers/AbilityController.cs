using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Abilities;
using PokeCraft.Cms.Core.Abilities.Models;
using PokeCraft.Cms.Models.Ability;

namespace PokeCraft.Cms.Controllers;

[ApiController]
[Route("api/abilities")]
public class AbilityController : ControllerBase
{
  private readonly IAbilityService _abilityService;

  public AbilityController(IAbilityService abilityService)
  {
    _abilityService = abilityService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Ability>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Ability? ability = await _abilityService.ReadAsync(id, key: null, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet("key:{key}")]
  public async Task<ActionResult<Ability>> ReadAsync(string key, CancellationToken cancellationToken)
  {
    Ability? ability = await _abilityService.ReadAsync(id: null, key, cancellationToken);
    return ability is null ? NotFound() : Ok(ability);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Ability>>> SearchAsync(SearchAbilitiesParameters parameters, CancellationToken cancellationToken)
  {
    SearchAbilitiesPayload payload = parameters.ToPayload();
    SearchResults<Ability> results = await _abilityService.SearchAsync(payload, cancellationToken);
    return Ok(results);
  }
}
