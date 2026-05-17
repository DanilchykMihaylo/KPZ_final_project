using System.Globalization;
using System.Windows.Data;
using Checkers.Models.Enums;

namespace Checkers.Converters
{
    public class PieceColorToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is PieceColor.Black;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}