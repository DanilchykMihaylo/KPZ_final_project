using Checkers.Models.Enums;

namespace Checkers.ViewModels
{
    public class PieceViewModel : BaseViewModel
    {
        private PieceColor _color;
        private PieceType _type;

        public PieceColor Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        public PieceType Type
        {
            get => _type;
            set => SetField(ref _type, value);
        }

        public bool IsKing => Type == PieceType.King;
    }
}