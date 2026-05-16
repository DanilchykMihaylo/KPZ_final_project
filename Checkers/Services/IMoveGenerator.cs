using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public interface IMoveGenerator
    {
        IReadOnlyList<Move> GetAvailableMoves(Board board, PieceColor color);
        IReadOnlyList<Move> GetMovesForPiece(Board board, Position position);
    }
}