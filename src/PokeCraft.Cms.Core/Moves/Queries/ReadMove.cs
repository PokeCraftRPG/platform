using Krakenar.Contracts;
using Logitar.CQRS;
using PokeCraft.Cms.Core.Moves.Models;

namespace PokeCraft.Cms.Core.Moves.Queries;

internal record ReadMoveQuery(Guid? Id, string? Key) : IQuery<Move?>;

internal class ReadMoveQueryHandler : IQueryHandler<ReadMoveQuery, Move?>
{
  private readonly IMoveQuerier _moveQuerier;

  public ReadMoveQueryHandler(IMoveQuerier moveQuerier)
  {
    _moveQuerier = moveQuerier;
  }

  public async Task<Move?> HandleAsync(ReadMoveQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Move> moves = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Move? move = await _moveQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Key))
    {
      Move? move = await _moveQuerier.ReadAsync(query.Key, cancellationToken);
      if (move is not null)
      {
        moves[move.Id] = move;
      }
    }

    if (moves.Count > 1)
    {
      throw TooManyResultsException<Move>.ExpectedSingle(moves.Count);
    }

    return moves.Values.SingleOrDefault();
  }
}
