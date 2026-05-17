using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public class MoveGenerator : IMoveGenerator
    {
        private readonly IMoveValidator _validator;

        public MoveGenerator(IMoveValidator validator)
        {
            _validator = validator;
        }

        public IReadOnlyList<Move> GetAvailableMoves(Board board, PieceColor color)
        {
            var pieces = board.GetPiecesByColor(color).ToList();

            var captures = pieces
                .SelectMany(p => GetCapturesForPiece(board, p.Position))
                .ToList();

            if (captures.Count > 0)
                return captures;

            return pieces
                .SelectMany(p => GetSimpleMovesForPiece(board, p.Position))
                .ToList();
        }

        public IReadOnlyList<Move> GetMovesForPiece(Board board, Position position)
        {
            var piece = board.GetPiece(position);
            if (piece is null) return [];

            var allCaptures = GetAvailableMoves(board, piece.Color)
                .Where(m => m.IsCapture)
                .ToList();

            if (allCaptures.Count > 0)
                return allCaptures.Where(m => m.From.Equals(position)).ToList();

            return GetSimpleMovesForPiece(board, position).ToList();
        }

        private IEnumerable<Move> GetSimpleMovesForPiece(Board board, Position from)
        {
            var piece = board.GetPiece(from);
            if (piece is null) yield break;

            if (piece.IsKing)
                foreach (var move in GetKingMoves(board, from))
                    yield return move;
            else
                foreach (var direction in GetManDirections(piece))
                {
                    var to = new Position(from.Row + direction.Row, from.Col + direction.Col);
                    var move = new Move(from, to);
                    if (_validator.IsValidMove(board, move))
                        yield return move;
                }
        }

        private IEnumerable<Move> GetKingMoves(Board board, Position from)
        {
            int[] dirs = [-1, 1];

            foreach (int rowDir in dirs)
                foreach (int colDir in dirs)
                {
                    int step = 1;
                    while (true)
                    {
                        var to = new Position(from.Row + rowDir * step, from.Col + colDir * step);
                        if (!to.IsWithinBounds(board.Size)) break;
                        if (!board.IsEmpty(to)) break;

                        var move = new Move(from, to);
                        if (_validator.IsValidMove(board, move))
                            yield return move;

                        step++;
                    }
                }
        }

        private IEnumerable<Move> GetCapturesForPiece(Board board, Position from)
        {
            var piece = board.GetPiece(from);
            if (piece is null) yield break;

            if (piece.IsKing)
                foreach (var move in GetKingCaptures(board, from))
                    yield return move;
            else
            {
                int[] deltas = [-1, 1];
                foreach (int dRow in deltas)
                    foreach (int dCol in deltas)
                    {
                        var captured = new Position(from.Row + dRow, from.Col + dCol);
                        var to = new Position(from.Row + dRow * 2, from.Col + dCol * 2);
                        var move = new Move(from, to, captured);
                        if (_validator.IsValidMove(board, move))
                            yield return move;
                    }
            }
        }

        private IEnumerable<Move> GetKingCaptures(Board board, Position from)
        {
            int[] dirs = [-1, 1];

            foreach (int rowDir in dirs)
                foreach (int colDir in dirs)
                {
                    Position? capturedPos = null;
                    int step = 1;

                    while (true)
                    {
                        var current = new Position(from.Row + rowDir * step, from.Col + colDir * step);
                        if (!current.IsWithinBounds(board.Size)) break;

                        var currentPiece = board.GetPiece(current);

                        if (currentPiece is not null)
                        {
                            var kingPiece = board.GetPiece(from)!;
                            if (currentPiece.Color == kingPiece.Color) break;
                            if (capturedPos is not null) break;
                            capturedPos = current;
                            step++;
                            continue;
                        }

                        if (capturedPos is not null)
                        {
                            var move = new Move(from, current, capturedPos);
                            if (_validator.IsValidMove(board, move))
                                yield return move;
                        }

                        step++;
                    }
                }
        }

        private static IEnumerable<Position> GetManDirections(Piece piece) =>
            piece.Color == PieceColor.White
                ? [new Position(-1, -1), new Position(-1, 1)]
                : [new Position(1, -1), new Position(1, 1)];
    }
}