using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace Pendar.Ums.Manager.Converters
{

    public class IdleToOpacityConverter : MarkupExtension, IValueConverter
    {
        public static IdleToOpacityConverter Instance = new IdleToOpacityConverter();

        public IdleToOpacityConverter()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((LineStatus)value == LineStatus.Idle)
            {
                return .3;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
