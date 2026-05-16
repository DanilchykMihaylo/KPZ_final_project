using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public interface IGameService
    {
        Board Board { get; }
        PieceColor CurrentTurn { get; }
        GameState GameState { get; }

        void StartNewGame();
        bool TryMakeMove(Move move);
        IReadOnlyList<Move> GetAvailableMovesForPiece(Position position);
    }
}