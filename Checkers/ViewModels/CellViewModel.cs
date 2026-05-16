namespace Checkers.ViewModels
{
    public class CellViewModel : BaseViewModel
    {
        private bool _isSelected;
        private bool _isHighlighted;
        private PieceViewModel? _piece;

        public int Row { get; }
        public int Col { get; }
        public bool IsDark { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetField(ref _isSelected, value);
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set => SetField(ref _isHighlighted, value);
        }

        public PieceViewModel? Piece
        {
            get => _piece;
            set
            {
                if (SetField(ref _piece, value))
                {
                    OnPropertyChanged(nameof(HasPiece));
                    OnPropertyChanged(nameof(PieceIsKing));
                }
            }
        }
        public bool HasPiece => _piece is not null;
        public bool PieceIsKing => _piece?.IsKing ?? false;

        public CellViewModel(int row, int col)
        {
            Row = row;
            Col = col;
            IsDark = (row + col) % 2 != 0;
        }
    }
}