using System;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class StringToTimeSpanConverter : IValueConverter
    {
        public static StringToTimeSpanConverter Instance = new StringToTimeSpanConverter();

        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t;
            if (TimeSpan.TryParse(value as string, out t))
                return t;
            else
                return TimeSpan.Zero;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return value.ToString();
            else
                return null;
        }
    }
}
