using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMSV;

namespace Pendar.Ums.Model.Converters
{
    public class InformingStatusConverter : IValueConverter
    {
        public static InformingStatusConverter Instance = new InformingStatusConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Folder.Utility.GetEnumDescription(((InformingStatus)value));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
