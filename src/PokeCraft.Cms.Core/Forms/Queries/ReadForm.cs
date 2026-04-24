using Krakenar.Contracts;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Forms.Models;

namespace PokeCraft.Cms.Core.Forms.Queries;

internal record ReadFormQuery(Guid? Id, string? Key) : IQuery<Form?>;

internal class ReadFormQueryHandler : IQueryHandler<ReadFormQuery, Form?>
{
  private readonly IFormQuerier _formQuerier;

  public ReadFormQueryHandler(IFormQuerier formQuerier)
  {
    _formQuerier = formQuerier;
  }

  public async Task<Form?> HandleAsync(ReadFormQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Form> forms = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Form? form = await _formQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (form is not null)
      {
        forms[form.Id] = form;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Key))
    {
      Form? form = await _formQuerier.ReadAsync(query.Key, cancellationToken);
      if (form is not null)
      {
        forms[form.Id] = form;
      }
    }

    if (forms.Count > 1)
    {
      throw TooManyResultsException<Form>.ExpectedSingle(forms.Count);
    }

    return forms.Values.SingleOrDefault();
  }
}
