using Krakenar.Contracts.Search;
using Microsoft.AspNetCore.Mvc;
using PokeCraft.Cms.Core.Forms;
using PokeCraft.Cms.Core.Forms.Models;
using PokeCraft.Cms.Models.Form;

namespace PokeCraft.Cms.Controllers;

[ApiController]
[Route("api/forms")]
public class FormController : ControllerBase
{
  private readonly IFormService _formService;

  public FormController(IFormService formService)
  {
    _formService = formService;
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Form>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Form? form = await _formService.ReadAsync(id, key: null, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet("key:{key}")]
  public async Task<ActionResult<Form>> ReadAsync(string key, CancellationToken cancellationToken)
  {
    Form? form = await _formService.ReadAsync(id: null, key, cancellationToken);
    return form is null ? NotFound() : Ok(form);
  }

  [HttpGet]
  public async Task<ActionResult<SearchResults<Form>>> SearchAsync(SearchFormsParameters parameters, CancellationToken cancellationToken)
  {
    SearchFormsPayload payload = parameters.ToPayload();
    SearchResults<Form> results = await _formService.SearchAsync(payload, cancellationToken);
    return Ok(results);
  }
}
