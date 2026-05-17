using Checkers.Models.Enums;

namespace Checkers.Models
{
    public class AppSettings
    {
        public int BoardSize { get; set; } = 8;
        public BoardTheme Theme { get; set; } = BoardTheme.Classic;
    }
}