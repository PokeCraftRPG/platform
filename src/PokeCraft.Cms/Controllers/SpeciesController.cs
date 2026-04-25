using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Species;
using PokeCraft.Cms.Core.Species.Models;
using PokeCraft.Cms.Models.Species;

namespace PokeCraft.Cms.Controllers;

[ApiController]
[Route("api/species")]
public class SpeciesController : ControllerBase
{
  private readonly ISpeciesService _speciesService;

  public SpeciesController(ISpeciesService speciesService)
  {
    _speciesService = speciesService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<PokemonSpecies>> ReadAsync(Guid id, bool expand, CancellationToken cancellationToken)
  {
    PokemonSpecies? species = await _speciesService.ReadAsync(id, key: null, number: null, expand, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("key:{key}")]
  public async Task<ActionResult<PokemonSpecies>> ReadAsync(string key, bool expand, CancellationToken cancellationToken)
  {
    PokemonSpecies? species = await _speciesService.ReadAsync(id: null, key, number: null, expand, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet("number:{number}")]
  public async Task<ActionResult<PokemonSpecies>> ReadAsync(int number, bool expand, CancellationToken cancellationToken)
  {
    PokemonSpecies? species = await _speciesService.ReadAsync(id: null, key: null, number, expand, cancellationToken);
    return species is null ? NotFound() : Ok(species);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<PokemonSpecies>>> SearchAsync(SearchSpeciesParameters parameters, CancellationToken cancellationToken)
  {
    SearchSpeciesPayload payload = parameters.ToPayload();
    SearchResults<PokemonSpecies> results = await _speciesService.SearchAsync(payload, cancellationToken);
    return Ok(results);
  }
}
