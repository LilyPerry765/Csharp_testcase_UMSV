using System;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class HexConverter : IValueConverter
    {
        public static HexConverter Instance = new HexConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int dec;
            if (int.TryParse(value.ToString(), out dec))
                return string.Format("{0:X}", dec);
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int dec;
            if (int.TryParse(value.ToString(), System.Globalization.NumberStyles.HexNumber, null, out dec))
                return dec;
            return null;
        }


    }
}
