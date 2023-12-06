using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class IntToBooleanConverter : MarkupExtension, IValueConverter
    {
        public static IntToBooleanConverter Instance = new IntToBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int?)value > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (((bool?)value) ?? false) ? 1 : 0;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
