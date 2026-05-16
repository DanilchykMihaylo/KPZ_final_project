using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    // Validates whether a given move is legal according to checkers rules.
    public class MoveValidator : IMoveValidator
    {
        public bool IsValidMove(Board board, Move move)
        {
            if (!move.From.IsWithinBounds() || !move.To.IsWithinBounds())
                return false;

            var piece = board.GetPiece(move.From);
            if (piece is null)
                return false;

            if (!board.IsEmpty(move.To))
                return false;

            return move.IsCapture
                ? IsValidCapture(board, move, piece)
                : IsValidSimpleMove(move, piece);
        }

        private static bool IsValidSimpleMove(Move move, Piece piece)
        {
            int rowDelta = move.To.Row - move.From.Row;
            int colDelta = Math.Abs(move.To.Col - move.From.Col);

            if (colDelta != 1)
                return false;

            if (piece.IsKing)
                return Math.Abs(rowDelta) == 1;

            return piece.Color == PieceColor.White
                ? rowDelta == -1
                : rowDelta == 1;
        }

        private static bool IsValidCapture(Board board, Move move, Piece piece)
        {
            if (move.CapturedPosition is null)
                return false;

            int rowDelta = move.To.Row - move.From.Row;
            int colDelta = move.To.Col - move.From.Col;

            if (Math.Abs(rowDelta) != 2 || Math.Abs(colDelta) != 2)
                return false;

            var captured = board.GetPiece(move.CapturedPosition);
            return captured is not null && captured.Color != piece.Color;
        }
    }
}