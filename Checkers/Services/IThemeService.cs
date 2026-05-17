using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public interface IThemeService
    {
        ThemeColors GetColors(BoardTheme theme);
    }
}