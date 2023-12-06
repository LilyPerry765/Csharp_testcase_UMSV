using System;
using System.Windows.Data;
using System.Windows.Markup;


namespace Pendar.Ums.Model.Converters
{
    public class NotConverter : MarkupExtension, IValueConverter
    {
        public static NotConverter Instance = new NotConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            return !(bool)value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
