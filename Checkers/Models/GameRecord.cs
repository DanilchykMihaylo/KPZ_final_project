using Checkers.Models.Enums;

namespace Checkers.Models
{
    public class GameRecord
    {
        public PieceColor CurrentTurn { get; set; }
        public GameState GameState { get; set; }
        public int ElapsedSeconds { get; set; }
        public int WhiteScore { get; set; }
        public int BlackScore { get; set; }
        public List<CellRecord> Cells { get; set; } = [];
    }

    public class CellRecord
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public PieceColor Color { get; set; }
        public PieceType Type { get; set; }
    }
}