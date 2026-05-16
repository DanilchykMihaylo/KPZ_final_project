namespace Checkers.ViewModels
{
    public class CellViewModel : BaseViewModel
    {
        private bool _isSelected;
        private bool _isHighlighted;
        private bool _isLastMoveFrom;
        private bool _isLastMoveTo;
        private bool _isForcedCapture;

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

        public bool IsLastMoveFrom
        {
            get => _isLastMoveFrom;
            set => SetField(ref _isLastMoveFrom, value);
        }

        public bool IsLastMoveTo
        {
            get => _isLastMoveTo;
            set => SetField(ref _isLastMoveTo, value);
        }

        public bool IsForcedCapture
        {
            get => _isForcedCapture;
            set => SetField(ref _isForcedCapture, value);
        }

        public CellViewModel(int row, int col)
        {
            Row = row;
            Col = col;
            IsDark = (row + col) % 2 != 0;
        }
    }
}