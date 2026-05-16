using Checkers.Models;

namespace Checkers.Services
{
    public interface IMoveValidator
    {
        bool IsValidMove(Board board, Move move);
    }
}