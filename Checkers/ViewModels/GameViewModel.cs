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
        private int _whitePieceCount;
        private int _blackPieceCount;
        private double _cellSize;
        private List<Move> _availableMoves = [];

        public ObservableCollection<CellViewModel> Cells { get; } = [];
        public ObservableCollection<PieceViewModel> Pieces { get; } = [];

        public ICommand CellClickCommand { get; }
        public ICommand NewGameCommand { get; }

        public string StatusMessage
        {
            get => _statusMessage;
            private set => SetField(ref _statusMessage, value);
        }

        public int WhitePieceCount
        {
            get => _whitePieceCount;
            private set => SetField(ref _whitePieceCount, value);
        }

        public int BlackPieceCount
        {
            get => _blackPieceCount;
            private set => SetField(ref _blackPieceCount, value);
        }

        public double CellSize
        {
            get => _cellSize;
            set
            {
                if (SetField(ref _cellSize, value))
                    RecalculatePiecePositions();
            }
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
            ClearSelection();
            ClearLastMoveHighlight();
            SyncPieces();
            UpdateStatus();
            UpdatePieceCounts();
            HighlightForcedCaptures();
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

            var moves = _gameService.GetAvailableMovesForPiece(new Position(cell.Row, cell.Col));
            if (moves.Count == 0)
                return;

            ClearSelection();

            _selectedCell = cell;
            cell.IsSelected = true;
            _availableMoves = moves.ToList();

            foreach (var move in _availableMoves)
                GetCell(move.To.Row, move.To.Col).IsHighlighted = true;
        }

        private void TryMakeMove(CellViewModel targetCell)
        {
            var isValidTarget = _availableMoves
                .Any(m => m.To.Row == targetCell.Row && m.To.Col == targetCell.Col);

            if (!isValidTarget)
            {
                ClearSelection();
                TrySelectPiece(targetCell);
                return;
            }

            var from = new Position(_selectedCell!.Row, _selectedCell.Col);
            var to = new Position(targetCell.Row, targetCell.Col);

            var movingPiece = Pieces.FirstOrDefault(p => p.Row == from.Row && p.Col == from.Col);

            ClearSelection();
            ClearLastMoveHighlight();

            GetCell(from.Row, from.Col).IsLastMoveFrom = true;
            GetCell(to.Row, to.Col).IsLastMoveTo = true;

            if (movingPiece is not null)
            {
                movingPiece.Row = to.Row;
                movingPiece.Col = to.Col;
                movingPiece.X = ColToX(to.Col);
                movingPiece.Y = RowToY(to.Row);
            }

            _gameService.TryMakeMove(new Move(from, to));

            SyncPieces();
            UpdateStatus();
            UpdatePieceCounts();
            HighlightForcedCaptures();
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

        private void ClearLastMoveHighlight()
        {
            foreach (var cell in Cells)
            {
                cell.IsLastMoveFrom = false;
                cell.IsLastMoveTo = false;
            }
        }

        private void HighlightForcedCaptures()
        {
            foreach (var cell in Cells)
                cell.IsForcedCapture = false;

            if (_gameService.GameState != GameState.InProgress)
                return;

            var allMoves = _gameService.GetAllAvailableMoves();
            if (!allMoves.Any(m => m.IsCapture))
                return;

            foreach (var pos in allMoves.Where(m => m.IsCapture).Select(m => m.From).Distinct())
                GetCell(pos.Row, pos.Col).IsForcedCapture = true;
        }

        private void SyncPieces()
        {
            var boardPieces = _gameService.Board.GetAllPieces().ToList();

            var toRemove = Pieces
                .Where(p => !boardPieces.Any(bp => bp.Position.Row == p.Row && bp.Position.Col == p.Col))
                .ToList();

            foreach (var piece in toRemove)
                Pieces.Remove(piece);

            foreach (var (position, boardPiece) in boardPieces)
            {
                var existing = Pieces.FirstOrDefault(p => p.Row == position.Row && p.Col == position.Col);

                if (existing is not null)
                {
                    existing.Color = boardPiece.Color;
                    existing.Type = boardPiece.Type;
                    continue;
                }

                var vm = new PieceViewModel
                {
                    Row = position.Row,
                    Col = position.Col,
                    Color = boardPiece.Color,
                    Type = boardPiece.Type,
                    X = ColToX(position.Col),
                    Y = RowToY(position.Row)
                };

                Pieces.Add(vm);
            }
        }

        private void RecalculatePiecePositions()
        {
            foreach (var piece in Pieces)
            {
                piece.X = ColToX(piece.Col);
                piece.Y = RowToY(piece.Row);
            }
        }

        private void UpdateStatus()
        {
            StatusMessage = _gameService.GameState switch
            {
                GameState.WhiteWins => " Білі перемогли!",
                GameState.BlackWins => " Чорні перемогли!",
                GameState.Draw => " Нічия!",
                _ => _gameService.CurrentTurn == PieceColor.White
                    ? "Хід: Білі "
                    : "Хід: Чорні "
            };
        }

        private void UpdatePieceCounts()
        {
            WhitePieceCount = _gameService.Board.GetPiecesByColor(PieceColor.White).Count();
            BlackPieceCount = _gameService.Board.GetPiecesByColor(PieceColor.Black).Count();
        }

        private double ColToX(int col) => col * _cellSize;
        private double RowToY(int row) => row * _cellSize;

        private CellViewModel GetCell(int row, int col) =>
            Cells[row * Board.Size + col];
    }
}