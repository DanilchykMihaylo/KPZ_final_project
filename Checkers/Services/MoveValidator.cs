using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
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
            int colDelta = move.To.Col - move.From.Col;

            if (Math.Abs(rowDelta) != Math.Abs(colDelta))
                return false;

            if (piece.IsKing)
                return true;

            if (Math.Abs(colDelta) != 1)
                return false;

            return piece.Color == PieceColor.White ? rowDelta == -1 : rowDelta == 1;
        }

        private static bool IsValidCapture(Board board, Move move, Piece piece)
        {
            if (move.CapturedPosition is null)
                return false;

            int rowDelta = move.To.Row - move.From.Row;
            int colDelta = move.To.Col - move.From.Col;

            if (Math.Abs(rowDelta) != Math.Abs(colDelta))
                return false;

            if (!piece.IsKing && (Math.Abs(rowDelta) != 2 || Math.Abs(colDelta) != 2))
                return false;

            var captured = board.GetPiece(move.CapturedPosition);
            if (captured is null || captured.Color == piece.Color)
                return false;

            return IsOnDiagonalBetween(move.From, move.To, move.CapturedPosition);
        }

        private static bool IsOnDiagonalBetween(Position from, Position to, Position candidate)
        {
            int rowDir = Math.Sign(to.Row - from.Row);
            int colDir = Math.Sign(to.Col - from.Col);

            var current = new Position(from.Row + rowDir, from.Col + colDir);

            while (!current.Equals(to))
            {
                if (current.Equals(candidate))
                    return true;
                current = new Position(current.Row + rowDir, current.Col + colDir);
            }

            return false;
        }
    }
}