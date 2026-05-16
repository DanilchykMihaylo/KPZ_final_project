using Checkers.Models.Enums;

namespace Checkers.ViewModels
{
    public class PieceViewModel : BaseViewModel
    {
        private PieceColor _color;
        private PieceType _type;
        private double _x;
        private double _y;
        private bool _isAnimating;

        public int Row { get; set; }
        public int Col { get; set; }

        public PieceColor Color
        {
            get => _color;
            set => SetField(ref _color, value);
        }

        public PieceType Type
        {
            get => _type;
            set
            {
                if (SetField(ref _type, value))
                    OnPropertyChanged(nameof(IsKing));
            }
        }

        public double X
        {
            get => _x;
            set => SetField(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => SetField(ref _y, value);
        }

        public bool IsAnimating
        {
            get => _isAnimating;
            set => SetField(ref _isAnimating, value);
        }

        public bool IsKing => Type == PieceType.King;
    }
}