using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class FormatConverter : IMultiValueConverter
    {
        public static FormatConverter Instance = new FormatConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (var item in values)
                if (item == null || item.ToString() == string.Empty)
                    return null;

            return string.Format((string)parameter ?? "{0}", values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
