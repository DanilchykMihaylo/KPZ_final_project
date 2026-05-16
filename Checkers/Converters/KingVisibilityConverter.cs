using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Checkers.Converters
{
    // Показує корону лише якщо Piece існує І є дамкою.
    public class KingVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is true ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}