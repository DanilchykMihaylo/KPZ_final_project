using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public interface IGameService
    {
        Board Board { get; }
        PieceColor CurrentTurn { get; }
        GameState GameState { get; }
        int WhiteScore { get; }
        int BlackScore { get; }

        void StartNewGame();
        void RestoreFromRecord(GameRecord record);
        GameRecord CreateRecord(int elapsedSeconds);
        bool TryMakeMove(Move move);
        IReadOnlyList<Move> GetAvailableMovesForPiece(Position position);
        IReadOnlyList<Move> GetAllAvailableMoves();
    }
}