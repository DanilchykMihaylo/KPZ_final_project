using Checkers.Models.Enums;

namespace Checkers.Models
{
    // Represents a single checker piece with color and type (Man or King).
    public class Piece
    {
        public PieceColor Color { get; }
        public PieceType Type { get; private set; }

        public bool IsKing => Type == PieceType.King;

        public Piece(PieceColor color, PieceType type = PieceType.Man)
        {
            Color = color;
            Type = type;
        }

        public void Promote() => Type = PieceType.King;

        public Piece Clone() => new Piece(Color, Type);
    }
}