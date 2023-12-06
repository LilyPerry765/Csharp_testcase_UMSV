using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Pendar.Ums.Model.Converters
{
    public class NullVisibilityConverter : MarkupExtension, IValueConverter
    {
        public static NullVisibilityConverter Instance = new NullVisibilityConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
