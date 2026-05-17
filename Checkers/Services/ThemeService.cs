using Checkers.Models;
using Checkers.Models.Enums;

namespace Checkers.Services
{
    public class ThemeService : IThemeService
    {
        private static readonly Dictionary<BoardTheme, ThemeColors> Themes = new()
        {
            [BoardTheme.Classic] = new ThemeColors
            {
                LightCell = "#F0E6D2",
                DarkCell = "#5C3317",
                WhitePiece = "#F0E6D2",
                BlackPiece = "#1E1008",
                Background = "#1A0E07",
                PanelBackground = "#2C1A0A"
            },
            [BoardTheme.Forest] = new ThemeColors
            {
                LightCell = "#D4E6C3",
                DarkCell = "#4A7C59",
                WhitePiece = "#E8F5E0",
                BlackPiece = "#1B3A2D",
                Background = "#0D1F15",
                PanelBackground = "#1B3A2D"
            },
            [BoardTheme.Ocean] = new ThemeColors
            {
                LightCell = "#C9E8F0",
                DarkCell = "#1A6B8A",
                WhitePiece = "#E0F4FA",
                BlackPiece = "#0D3B52",
                Background = "#071D2B",
                PanelBackground = "#0D3B52"
            },
            [BoardTheme.Night] = new ThemeColors
            {
                LightCell = "#3A3A4A",
                DarkCell = "#1A1A2A",
                WhitePiece = "#C0C0D0",
                BlackPiece = "#101018",
                Background = "#080810",
                PanelBackground = "#1A1A2A"
            }
        };

        public ThemeColors GetColors(BoardTheme theme) => Themes[theme];
    }
}