namespace Checkers.Models
{
    // Represents a move: from one position to another,
    // with an optional captured piece position.
    public class Move
    {
        public Position From { get; }
        public Position To { get; }
        public Position? CapturedPosition { get; }

        public bool IsCapture => CapturedPosition is not null;

        public Move(Position from, Position to, Position? capturedPosition = null)
        {
            From = from;
            To = to;
            CapturedPosition = capturedPosition;
        }

        public override string ToString() =>
            IsCapture
                ? $"{From} -> {To} (captures {CapturedPosition})"
                : $"{From} -> {To}";
    }
}