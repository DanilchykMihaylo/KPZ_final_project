using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public class GameService : IGameService
    {
        private readonly IMoveGenerator _moveGenerator;
        private int _boardSize;

        public Board Board { get; private set; } = new Board();
        public PieceColor CurrentTurn { get; private set; } = PieceColor.White;
        public GameState GameState { get; private set; } = GameState.InProgress;
        public int WhiteScore { get; private set; }
        public int BlackScore { get; private set; }

        public GameService(IMoveGenerator moveGenerator, int boardSize = 8)
        {
            _moveGenerator = moveGenerator;
            _boardSize = boardSize;
        }

        public void SetBoardSize(int size) => _boardSize = size;

        public void StartNewGame()
        {
            Board = new Board(_boardSize);
            Board.InitializeStartingPosition();
            CurrentTurn = PieceColor.White;
            GameState = GameState.InProgress;
        }

        public void RestoreFromRecord(GameRecord record)
        {
            Board = new Board(record.BoardSize);
            foreach (var cell in record.Cells)
                Board.SetPiece(new Position(cell.Row, cell.Col), new Piece(cell.Color, cell.Type));

            CurrentTurn = record.CurrentTurn;
            GameState = record.GameState;
            WhiteScore = record.WhiteScore;
            BlackScore = record.BlackScore;
            _boardSize = record.BoardSize;
        }

        public GameRecord CreateRecord(int elapsedSeconds) => new()
        {
            CurrentTurn = CurrentTurn,
            GameState = GameState,
            ElapsedSeconds = elapsedSeconds,
            WhiteScore = WhiteScore,
            BlackScore = BlackScore,
            BoardSize = _boardSize,
            Cells = Board.GetAllPieces()
                .Select(p => new CellRecord
                {
                    Row = p.Position.Row,
                    Col = p.Position.Col,
                    Color = p.Piece.Color,
                    Type = p.Piece.Type
                }).ToList()
        };

        public bool TryMakeMove(Move move)
        {
            if (GameState != GameState.InProgress)
                return false;

            var available = _moveGenerator.GetMovesForPiece(Board, move.From);
            var matched = available.FirstOrDefault(m =>
                m.From.Equals(move.From) && m.To.Equals(move.To));

            if (matched is null)
                return false;

            ApplyMove(matched);
            return true;
        }

        public IReadOnlyList<Move> GetAvailableMovesForPiece(Position position) =>
            _moveGenerator.GetMovesForPiece(Board, position);

        public IReadOnlyList<Move> GetAllAvailableMoves() =>
            _moveGenerator.GetAvailableMoves(Board, CurrentTurn);

        private void ApplyMove(Move move)
        {
            Board.MovePiece(move.From, move.To);

            if (move.IsCapture)
                Board.RemovePiece(move.CapturedPosition!);

            TryPromote(move.To);

            bool canCaptureAgain = move.IsCapture &&
                _moveGenerator.GetMovesForPiece(Board, move.To).Any(m => m.IsCapture);

            if (!canCaptureAgain)
                SwitchTurn();

            UpdateGameState();
        }

        private void TryPromote(Position position)
        {
            var piece = Board.GetPiece(position);
            if (piece is null || piece.IsKing) return;

            bool shouldPromote = piece.Color == PieceColor.White
                ? position.Row == 0
                : position.Row == Board.Size - 1;

            if (shouldPromote)
                piece.Promote();
        }

        private void SwitchTurn() =>
            CurrentTurn = CurrentTurn == PieceColor.White
                ? PieceColor.Black
                : PieceColor.White;

        private void UpdateGameState()
        {
            var whiteMoves = _moveGenerator.GetAvailableMoves(Board, PieceColor.White);
            var blackMoves = _moveGenerator.GetAvailableMoves(Board, PieceColor.Black);

            bool whiteHasPieces = Board.GetPiecesByColor(PieceColor.White).Any();
            bool blackHasPieces = Board.GetPiecesByColor(PieceColor.Black).Any();

            var newState = (whiteHasPieces, blackHasPieces, whiteMoves.Count, blackMoves.Count) switch
            {
                (false, _, _, _) => GameState.BlackWins,
                (_, false, _, _) => GameState.WhiteWins,
                ({ }, { }, 0, _) when CurrentTurn == PieceColor.White => GameState.BlackWins,
                ({ }, { }, _, 0) when CurrentTurn == PieceColor.Black => GameState.WhiteWins,
                _ => GameState.InProgress
            };

            if (newState != GameState.InProgress && GameState == GameState.InProgress)
            {
                if (newState == GameState.WhiteWins) WhiteScore++;
                if (newState == GameState.BlackWins) BlackScore++;
            }

            GameState = newState;
        }
    }
}