using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMSV;

namespace UMSV
{
    public class GraphTooltipConverter : IValueConverter
    {

        public static GraphTooltipConverter Instance = new GraphTooltipConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Graph graph = value as Graph;
            if (graph != null)
            {
                return graph.Name;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GraphIDToGraphNameConverter : IValueConverter
    {
        public static GraphIDToGraphNameConverter Instance = new GraphIDToGraphNameConverter();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                Graph graph = context.Graphs.Where(t => t.ID == Guid.Parse(value.ToString())).SingleOrDefault();

                return graph == null ? string.Empty : graph.Name;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
