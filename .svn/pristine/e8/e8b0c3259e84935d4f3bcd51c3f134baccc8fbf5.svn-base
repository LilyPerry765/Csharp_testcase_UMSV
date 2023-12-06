using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;

namespace Pendar.Ums.Model.Converters
{
    public class IfConverter : MarkupExtension, IValueConverter
    {
        public static IfConverter Instance = new IfConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(bool?))
                return ToBoolean(value, parameter as IfConverterParameter);
            else
                return FromBoolean(value as bool?, parameter as IfConverterParameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType == typeof(bool))
                return ToBoolean(value, parameter as IfConverterParameter);
            else
                return FromBoolean(value as bool?, parameter as IfConverterParameter);
        }

        private bool ToBoolean(object value, IfConverterParameter param)
        {
            if (value == null)
                return value == param.TrueValue;
            else
                return value.Equals(param.TrueValue);
        }

        private object FromBoolean(bool? value, IfConverterParameter param)
        {
            return (value ?? false) ? param.TrueValue : param.FalseValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

    }

    public class IfConverterParameter : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object FalseValue
        {
            get;
            set;
        }

        public object TrueValue
        {
            get;
            set;
        }
    }

}
