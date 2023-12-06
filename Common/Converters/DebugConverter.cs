using System;
using System.Windows.Markup;
using System.Windows.Data;
using System.Diagnostics;

namespace Pendar.Ums.Model.Converters
{
    public class DebugConverter : MarkupExtension, IValueConverter
    {
        public static DebugConverter Instance = new DebugConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
