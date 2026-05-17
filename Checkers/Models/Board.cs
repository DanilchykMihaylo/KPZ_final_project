using Checkers.Models.Enums;

namespace Checkers.Models
{
    public class Board
    {
        public static int DefaultSize => 8;
        public int Size { get; }

        private readonly Piece?[,] _cells;

        public Board(int size = 8)
        {
            Size = size;
            _cells = new Piece?[Size, Size];
        }

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
            var clone = new Board(Size);
            foreach (var (position, piece) in GetAllPieces())
                clone.SetPiece(position, piece.Clone());
            return clone;
        }

        public void InitializeStartingPosition()
        {
            for (int row = 0; row < Size; row++)
                for (int col = 0; col < Size; col++)
                    _cells[row, col] = null;

            int rows = (Size / 2) - 1;

            PlacePiecesForColor(PieceColor.Black, 0, rows - 1);
            PlacePiecesForColor(PieceColor.White, Size - rows, Size - 1);
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