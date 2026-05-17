namespace Checkers.Models
{
    public sealed class Position
    {
        public int Row { get; }
        public int Col { get; }

        public Position(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public bool IsWithinBounds(int boardSize) =>
            Row >= 0 && Row < boardSize &&
            Col >= 0 && Col < boardSize;

        public override bool Equals(object? obj) =>
            obj is Position other && Row == other.Row && Col == other.Col;

        public override int GetHashCode() => HashCode.Combine(Row, Col);

        public override string ToString() => $"({Row}, {Col})";
    }
}