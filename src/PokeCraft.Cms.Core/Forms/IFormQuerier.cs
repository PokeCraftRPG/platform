using Krakenar.Contracts.Search;
using PokeCraft.Cms.Core.Forms.Models;

namespace PokeCraft.Cms.Core.Forms;

public interface IFormQuerier
{
  Task<Form?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Form?> ReadAsync(string key, CancellationToken cancellationToken = default);
  Task<SearchResults<Form>> SearchAsync(SearchFormsPayload payload, CancellationToken cancellationToken = default);
}
