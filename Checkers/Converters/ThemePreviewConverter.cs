using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Checkers.Models.Enums;

namespace Checkers.Converters
{
    public class ThemePreviewConverter : IValueConverter
    {
        private static readonly Dictionary<BoardTheme, (string Light, string Dark)> Colors = new()
        {
            [BoardTheme.Classic] = ("#F0E6D2", "#5C3317"),
            [BoardTheme.Forest] = ("#D4E6C3", "#4A7C59"),
            [BoardTheme.Ocean] = ("#C9E8F0", "#1A6B8A"),
            [BoardTheme.Night] = ("#3A3A4A", "#1A1A2A"),
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not BoardTheme theme) return Brushes.Transparent;
            if (parameter is not string param) return Brushes.Transparent;

            var (light, dark) = Colors[theme];
            var hex = param == "light" ? light : dark;

            return new SolidColorBrush(
                (Color)ColorConverter.ConvertFromString(hex));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}