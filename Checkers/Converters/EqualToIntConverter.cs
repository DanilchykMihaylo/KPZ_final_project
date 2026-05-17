using System.Globalization;
using System.Windows.Data;

namespace Checkers.Converters
{
    public class EqualToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue && parameter is string paramStr &&
                int.TryParse(paramStr, out int paramValue))
                return intValue == paramValue;

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is true && parameter is string paramStr &&
                int.TryParse(paramStr, out int paramValue))
                return paramValue;

            return Binding.DoNothing;
        }
    }
}