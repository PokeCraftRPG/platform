using Krakenar.Contracts;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Varieties.Models;

namespace PokeCraft.Cms.Core.Varieties.Queries;

internal record ReadVarietyQuery(Guid? Id, string? Key) : IQuery<Variety?>;

internal class ReadVarietyQueryHandler : IQueryHandler<ReadVarietyQuery, Variety?>
{
  private readonly IVarietyQuerier _varietyQuerier;

  public ReadVarietyQueryHandler(IVarietyQuerier varietyQuerier)
  {
    _varietyQuerier = varietyQuerier;
  }

  public async Task<Variety?> HandleAsync(ReadVarietyQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Variety> varieties = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Variety? variety = await _varietyQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (variety is not null)
      {
        varieties[variety.Id] = variety;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Key))
    {
      Variety? variety = await _varietyQuerier.ReadAsync(query.Key, cancellationToken);
      if (variety is not null)
      {
        varieties[variety.Id] = variety;
      }
    }

    if (varieties.Count > 1)
    {
      throw TooManyResultsException<Variety>.ExpectedSingle(varieties.Count);
    }

    return varieties.Values.SingleOrDefault();
  }
}
