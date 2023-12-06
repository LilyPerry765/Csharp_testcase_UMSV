using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class UniformMultiConverter : MarkupExtension, IMultiValueConverter
    {
        public static UniformMultiConverter Instance = new UniformMultiConverter();

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object first = values.FirstOrDefault();
            foreach (var item in values)
                if (!((item == null && first == null) || item.Equals(first)))
                    throw new Exception("All values must be the same to use uniform converter.");
            return first;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            object[] result = new object[targetTypes.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = value;
            }
            return result;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }
    }
}
