using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class NullToBooleanConverter : MarkupExtension, IValueConverter
    {
        public static NullToBooleanConverter Instance = new NullToBooleanConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
                return string.IsNullOrEmpty(value as string) ? false : true;
            else
                return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
