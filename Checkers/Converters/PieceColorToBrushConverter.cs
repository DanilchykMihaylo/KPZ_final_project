using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Checkers.Models.Enums;

namespace Checkers.Converters
{
    public class PieceColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PieceColor color)
            {
                return color == PieceColor.White
                    ? new SolidColorBrush(Color.FromRgb(240, 230, 210))
                    : new SolidColorBrush(Color.FromRgb(50, 30, 10));
            }

            return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}