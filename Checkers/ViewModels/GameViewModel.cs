using System.Collections.ObjectModel;
using System.Windows.Input;
using Checkers.Models;
using Checkers.Models.Enums;
using Checkers.Services;

namespace Checkers.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly IGameService _gameService;

        private CellViewModel? _selectedCell;
        private string _statusMessage = string.Empty;
        private List<Move> _availableMoves = [];

        public ObservableCollection<CellViewModel> Cells { get; } = [];
        public ICommand CellClickCommand { get; }
        public ICommand NewGameCommand { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetField(ref _statusMessage, value);
        }

        public GameViewModel(IGameService gameService)
        {
            _gameService = gameService;

            CellClickCommand = new RelayCommand<CellViewModel>(OnCellClicked);
            NewGameCommand = new RelayCommand(OnNewGame);

            InitializeCells();
            StartGame();
        }

        private void InitializeCells()
        {
            for (int row = 0; row < Board.Size; row++)
                for (int col = 0; col < Board.Size; col++)
                    Cells.Add(new CellViewModel(row, col));
        }

        private void StartGame()
        {
            _gameService.StartNewGame();
            SyncBoardState();
            UpdateStatus();
        }

        private void OnNewGame() => StartGame();

        private void OnCellClicked(CellViewModel? cell)
        {
            if (cell is null || _gameService.GameState != GameState.InProgress)
                return;

            if (_selectedCell is null)
            {
                TrySelectPiece(cell);
            }
            else if (_selectedCell.Equals(cell))
            {
                ClearSelection();
            }
            else
            {
                TryMakeMove(cell);
            }
        }

        private void TrySelectPiece(CellViewModel cell)
        {
            var piece = _gameService.Board.GetPiece(new Position(cell.Row, cell.Col));
            if (piece is null || piece.Color != _gameService.CurrentTurn)
                return;

            ClearSelection();

            _selectedCell = cell;
            cell.IsSelected = true;

            _availableMoves = _gameService
                .GetAvailableMovesForPiece(new Position(cell.Row, cell.Col))
                .ToList();

            foreach (var move in _availableMoves)
                GetCell(move.To.Row, move.To.Col).IsHighlighted = true;
        }

        private void TryMakeMove(CellViewModel targetCell)
        {
            var from = new Position(_selectedCell!.Row, _selectedCell.Col);
            var to = new Position(targetCell.Row, targetCell.Col);
            var move = new Move(from, to);

            bool success = _gameService.TryMakeMove(move);
            ClearSelection();

            if (success)
            {
                SyncBoardState();
                UpdateStatus();
            }
        }

        private void ClearSelection()
        {
            if (_selectedCell is not null)
                _selectedCell.IsSelected = false;

            foreach (var move in _availableMoves)
                GetCell(move.To.Row, move.To.Col).IsHighlighted = false;

            _selectedCell = null;
            _availableMoves.Clear();
        }

        private void SyncBoardState()
        {
            foreach (var cell in Cells)
            {
                var boardPiece = _gameService.Board.GetPiece(new Position(cell.Row, cell.Col));
                cell.Piece = boardPiece is null ? null : new PieceViewModel
                {
                    Color = boardPiece.Color,
                    Type = boardPiece.Type
                };
            }
        }

        private void UpdateStatus()
        {
            StatusMessage = _gameService.GameState switch
            {
                GameState.WhiteWins => "Білі перемогли!",
                GameState.BlackWins => "Чорні перемогли!",
                GameState.Draw => "Нічия!",
                _ => _gameService.CurrentTurn == PieceColor.White
                    ? "Хід: Білі "
                    : "Хід: Чорні "
            };
        }

        private CellViewModel GetCell(int row, int col) =>
            Cells[row * Board.Size + col];
    }
}