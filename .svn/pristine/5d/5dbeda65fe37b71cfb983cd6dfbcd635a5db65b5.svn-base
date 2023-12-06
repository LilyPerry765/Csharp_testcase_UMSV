using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;

namespace Pendar.Ums.Model.Converters
{
    public class IgnoreNewItemPlaceHolderConverter : MarkupExtension, IValueConverter
    {
        private const string NewItemPlaceholderName = "{NewItemPlaceholder}";
        public static IgnoreNewItemPlaceHolderConverter Instance = new IgnoreNewItemPlaceHolderConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && (value.ToString() == NewItemPlaceholderName || value == DependencyProperty.UnsetValue))
                return null;
            //return DependencyProperty.UnsetValue;
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
