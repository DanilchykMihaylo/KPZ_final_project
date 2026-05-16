using Checkers.Models.Enums;

namespace Checkers.Models
{
    // Represents the 8x8 checkers board and manages piece placement.

    public class Board
    {
        public const int Size = 8;

        private readonly Piece?[,] _cells = new Piece?[Size, Size];

        public Piece? GetPiece(Position position) =>
            _cells[position.Row, position.Col];

        public void SetPiece(Position position, Piece? piece) =>
            _cells[position.Row, position.Col] = piece;

        public bool IsEmpty(Position position) =>
            GetPiece(position) is null;

        public void MovePiece(Position from, Position to)
        {
            var piece = GetPiece(from)
                ?? throw new InvalidOperationException($"No piece at {from}");

            SetPiece(to, piece);
            SetPiece(from, null);
        }

        public void RemovePiece(Position position) =>
            SetPiece(position, null);

        public IEnumerable<(Position Position, Piece Piece)> GetAllPieces()
        {
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                {
                    var position = new Position(row, col);
                    var piece = GetPiece(position);
                    if (piece is not null)
                        yield return (position, piece);
                }
        }

        public IEnumerable<(Position Position, Piece Piece)> GetPiecesByColor(PieceColor color) =>
            GetAllPieces().Where(x => x.Piece.Color == color);

        public Board Clone()
        {
            var clone = new Board();
            foreach (var (position, piece) in GetAllPieces())
                clone.SetPiece(position, piece.Clone());
            return clone;
        }

        // Sets up the standard starting position for checkers.
        // Black pieces occupy rows 0–2, white pieces occupy rows 5–7.

        public void InitializeStartingPosition()
        {
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    _cells[row, col] = null;

            PlacePiecesForColor(PieceColor.Black, startRow: 0, endRow: 2);
            PlacePiecesForColor(PieceColor.White, startRow: 5, endRow: 7);
        }

        private void PlacePiecesForColor(PieceColor color, int startRow, int endRow)
        {
            for (int row = startRow; row <= endRow; row++)
                for (int col = 0; col < Size; col++)
                    if (IsDarkCell(row, col))
                        SetPiece(new Position(row, col), new Piece(color));
        }

        private static bool IsDarkCell(int row, int col) =>
            (row + col) % 2 != 0;
    }
}