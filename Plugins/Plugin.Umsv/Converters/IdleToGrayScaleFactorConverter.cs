using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using Pendar.Ums.Manager.UmsEngine;

namespace Pendar.Ums.Manager.Converters
{

    public class IdleToGrayScaleFactorConverter : MarkupExtension, IValueConverter
    {
        public static IdleToGrayScaleFactorConverter Instance = new IdleToGrayScaleFactorConverter();

        public IdleToGrayScaleFactorConverter()
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
                return 0;
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
