using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using Pendar.Ums.Manager.UmsEngine;

namespace Pendar.Ums.Manager.Converters
{
    public class LineStatusConverter : MarkupExtension, IValueConverter
    {
        public static LineStatusConverter Instance = new LineStatusConverter();

        public LineStatusConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((LineStatus)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
