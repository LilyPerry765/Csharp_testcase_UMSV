using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using UMSV;

namespace Pendar.Ums.Model.Converters
{
    public class VoiceListItemConverter : MarkupExtension, IValueConverter
    {
        public static VoiceListItemConverter Instance = new VoiceListItemConverter();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(value as string))
                return "(صدای ضبط شده)";
            else if ((string)value == Constants.InfoTableRecordDataPropertyName)
                return "...";
            else
                return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
