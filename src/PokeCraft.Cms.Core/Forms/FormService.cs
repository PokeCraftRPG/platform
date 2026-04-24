using Krakenar.Contracts.Search;
using Logitar.CQRS;
using Microsoft.Extensions.DependencyInjection;
using PokeCraft.Cms.Core.Forms.Models;
using PokeCraft.Cms.Core.Forms.Queries;

namespace PokeCraft.Cms.Core.Forms;

public interface IFormService
{
  Task<Form?> ReadAsync(Guid? id = null, string? key = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Form>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken = default);
}

internal class FormService : IFormService
{
  public static void Register(IServiceCollection services)
  {
    services.AddTransient<IFormService, FormService>();
    services.AddTransient<IQueryHandler<ReadFormQuery, Form?>, ReadFormQueryHandler>();
    services.AddTransient<IQueryHandler<SearchFormsQuery, SearchResults<Form>>, SearchFormsQueryHandler>();
  }

  private readonly IQueryBus _queryBus;

  public FormService(IQueryBus queryBus)
  {
    _queryBus = queryBus;
  }

  public async Task<Form?> ReadAsync(Guid? id, string? key, CancellationToken cancellationToken)
  {
    ReadFormQuery query = new(id, key);
    return await _queryBus.ExecuteAsync(query, cancellationToken);
  }

  public Task<SearchResults<Form>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken)
  {
    SearchFormsQuery query = new(payload);
    return _queryBus.ExecuteAsync(query, cancellationToken);
  }
}
