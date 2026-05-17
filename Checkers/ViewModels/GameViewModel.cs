using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Checkers.Models;
using Checkers.Models.Enums;
using Checkers.Services;

namespace Checkers.ViewModels
{
    public class GameViewModel : BaseViewModel
    {
        private readonly IGameService _gameService;
        private readonly IGamePersistenceService _persistenceService;
        private readonly ISettingsService _settingsService;
        private readonly IThemeService _themeService;
        private readonly DispatcherTimer _timer;

        private CellViewModel? _selectedCell;
        private string _statusMessage = string.Empty;
        private int _whitePieceCount;
        private int _blackPieceCount;
        private int _elapsedSeconds;
        private bool _canLoad;
        private double _cellSize;
        private int _boardSize;
        private ThemeColors _themeColors = new();
        private List<Move> _availableMoves = [];

        public ObservableCollection<CellViewModel> Cells { get; } = [];
        public ObservableCollection<PieceViewModel> Pieces { get; } = [];

        public ICommand CellClickCommand { get; }
        public ICommand NewGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }
        public ICommand OpenSettingsCommand { get; }

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

        public int ElapsedSeconds
        {
            get => _elapsedSeconds;
            private set
            {
                if (SetField(ref _elapsedSeconds, value))
                    OnPropertyChanged(nameof(TimerDisplay));
            }
        }

        public string TimerDisplay =>
            TimeSpan.FromSeconds(_elapsedSeconds).ToString(@"mm\:ss");

        public bool CanLoad
        {
            get => _canLoad;
            private set => SetField(ref _canLoad, value);
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

        public int BoardSize
        {
            get => _boardSize;
            private set => SetField(ref _boardSize, value);
        }

        public int WhiteScore => _gameService.WhiteScore;
        public int BlackScore => _gameService.BlackScore;

        public Brush LightCellBrush => BrushFrom(_themeColors.LightCell);
        public Brush DarkCellBrush => BrushFrom(_themeColors.DarkCell);
        public Brush WhitePieceBrush => BrushFrom(_themeColors.WhitePiece);
        public Brush BlackPieceBrush => BrushFrom(_themeColors.BlackPiece);
        public Brush BackgroundBrush => BrushFrom(_themeColors.Background);
        public Brush PanelBrush => BrushFrom(_themeColors.PanelBackground);

        public event Action? SettingsRequested;

        public GameViewModel(
            IGameService gameService,
            IGamePersistenceService persistenceService,
            ISettingsService settingsService,
            IThemeService themeService)
        {
            _gameService = gameService;
            _persistenceService = persistenceService;
            _settingsService = settingsService;
            _themeService = themeService;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (_, _) => ElapsedSeconds++;

            CellClickCommand = new RelayCommand<CellViewModel>(OnCellClicked);
            NewGameCommand = new RelayCommand(OnNewGame);
            SaveGameCommand = new RelayCommand(OnSaveGame);
            LoadGameCommand = new RelayCommand(OnLoadGame);
            OpenSettingsCommand = new RelayCommand(OnOpenSettings);

            ApplySettings(_settingsService.Current);
            InitializeCells();
            CanLoad = _persistenceService.SaveExists();
            StartGame();
        }

        public void ApplySettings(AppSettings settings)
        {
            _themeColors = _themeService.GetColors(settings.Theme);
            _boardSize = settings.BoardSize;
            _gameService.SetBoardSize(settings.BoardSize);

            NotifyThemeChanged();
            BoardSize = settings.BoardSize;

            Cells.Clear();
            Pieces.Clear();
            InitializeCells();

            StartGame();
        }

        private void RebuildCells()
        {
            Cells.Clear();
            InitializeCells();
        }

        private void InitializeCells()
        {
            for (int row = 0; row < _boardSize; row++)
                for (int col = 0; col < _boardSize; col++)
                    Cells.Add(new CellViewModel(row, col));
        }

        private void NotifyThemeChanged()
        {
            OnPropertyChanged(nameof(LightCellBrush));
            OnPropertyChanged(nameof(DarkCellBrush));
            OnPropertyChanged(nameof(WhitePieceBrush));
            OnPropertyChanged(nameof(BlackPieceBrush));
            OnPropertyChanged(nameof(BackgroundBrush));
            OnPropertyChanged(nameof(PanelBrush));
        }

        private void StartGame()
        {
            _gameService.StartNewGame();
            ElapsedSeconds = 0;
            _timer.Start();
            ClearSelection();
            ClearLastMoveHighlight();
            SyncPieces();
            UpdateStatus();
            UpdatePieceCounts();
            UpdateScores();
            HighlightForcedCaptures();
        }

        private void OnNewGame() => StartGame();
        private void OnOpenSettings() => SettingsRequested?.Invoke();

        private void OnSaveGame()
        {
            var record = _gameService.CreateRecord(ElapsedSeconds);
            _persistenceService.Save(record);
            CanLoad = true;
        }

        private void OnLoadGame()
        {
            var record = _persistenceService.Load();
            if (record is null) return;

            _gameService.RestoreFromRecord(record);
            ElapsedSeconds = record.ElapsedSeconds;
            _boardSize = record.BoardSize;
            BoardSize = record.BoardSize;

            RebuildCells();
            _timer.Start();
            ClearSelection();
            ClearLastMoveHighlight();
            SyncPieces();
            UpdateStatus();
            UpdatePieceCounts();
            UpdateScores();
            HighlightForcedCaptures();
        }

        private void OnCellClicked(CellViewModel? cell)
        {
            if (cell is null || _gameService.GameState != GameState.InProgress)
                return;

            if (_selectedCell is null)
                TrySelectPiece(cell);
            else if (_selectedCell.Equals(cell))
                ClearSelection();
            else
                TryMakeMove(cell);
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
            UpdateScores();
            HighlightForcedCaptures();

            if (_gameService.GameState != GameState.InProgress)
                _timer.Stop();
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
                var existing = Pieces.FirstOrDefault(p =>
                    p.Row == position.Row && p.Col == position.Col);

                if (existing is not null)
                {
                    existing.Color = boardPiece.Color;
                    existing.Type = boardPiece.Type;
                    continue;
                }

                Pieces.Add(new PieceViewModel
                {
                    Row = position.Row,
                    Col = position.Col,
                    Color = boardPiece.Color,
                    Type = boardPiece.Type,
                    X = ColToX(position.Col),
                    Y = RowToY(position.Row)
                });
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

        private void UpdateScores()
        {
            OnPropertyChanged(nameof(WhiteScore));
            OnPropertyChanged(nameof(BlackScore));
        }

        private double ColToX(int col) => col * _cellSize;
        private double RowToY(int row) => row * _cellSize;

        private CellViewModel GetCell(int row, int col) =>
            Cells[row * _boardSize + col];

        private static SolidColorBrush BrushFrom(string hex) =>
            new(ColorConverter.ConvertFromString(hex) is Color c ? c : Colors.Transparent);
    }
}