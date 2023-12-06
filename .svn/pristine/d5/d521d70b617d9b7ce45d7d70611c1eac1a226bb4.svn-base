using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using Folder;


namespace Pendar.Ums.Model.Converters
{
    public class PersianDateConverter : IValueConverter
    {
        //public static PersianDateConverter Instance = new PersianDateConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime? date = value as DateTime?;
            if (!date.HasValue || !IsInValidRange(date.Value))
                return "-";

            return new PersianDateTime(date.Value).ToString((string)parameter ?? "yyyy/MM/dd");
        }

        private bool IsInValidRange(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return date >= pc.MinSupportedDateTime && date <= pc.MaxSupportedDateTime;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PersianDateTime p;
            if (PersianDateTime.TryParse(value as string, out p))
            {
                return p.ToGregorian();
            }
            return null;
        }

    }

    public class PersianDateMultiValueConverter : IMultiValueConverter
    {
        public static PersianDateMultiValueConverter Instance = new PersianDateMultiValueConverter();

        //PersianDateMultiValueConverter()
        //{
        //}

        private bool IsInValidRange(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            return date >= pc.MinSupportedDateTime && date <= pc.MaxSupportedDateTime;
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> persianDates = new List<string>();
            foreach (object value in values)
            {
                DateTime? date = value as DateTime?;
                if (!date.HasValue || !IsInValidRange(date.Value))
                    continue;

                persianDates.Add(new PersianDateTime(date.Value).Date.ToString());
            }
            return string.Format((string)parameter ?? "{0}", persianDates.ToArray());
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
