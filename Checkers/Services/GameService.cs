using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    // Orchestrates the game: applies moves, enforces rules,
    // handles promotions, and determines game-over conditions.
    public class GameService : IGameService
    {
        private readonly IMoveGenerator _moveGenerator;

        public Board Board { get; private set; } = new Board();
        public PieceColor CurrentTurn { get; private set; } = PieceColor.White;
        public GameState GameState { get; private set; } = GameState.InProgress;

        public GameService(IMoveGenerator moveGenerator)
        {
            _moveGenerator = moveGenerator;
        }

        public void StartNewGame()
        {
            Board = new Board();
            Board.InitializeStartingPosition();
            CurrentTurn = PieceColor.White;
            GameState = GameState.InProgress;
        }

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

            GameState = (whiteHasPieces, blackHasPieces, whiteMoves.Count, blackMoves.Count) switch
            {
                (false, _, _, _) or (_, _, 0, _) when CurrentTurn == PieceColor.White
                    => GameState.BlackWins,
                (_, false, _, _) or (_, _, _, 0) when CurrentTurn == PieceColor.Black
                    => GameState.WhiteWins,
                _ => GameState.InProgress
            };
        }
    }
}