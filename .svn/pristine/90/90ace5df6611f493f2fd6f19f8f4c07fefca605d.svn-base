using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using Pendar.Ums.Manager.UmsEngine;
using Folder;

namespace Pendar.Ums.Manager.Converters
{
    public class LineStatusImageConverter : MarkupExtension, IValueConverter
    {
        public static LineStatusImageConverter Instance = new LineStatusImageConverter();

        public LineStatusImageConverter()
        {
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Instance;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = "transparent.png";
            switch ((LineStatus)value)
            {
                //case LineStatus.Connect:
                //    path = "connect.png";
                //    break;
                case LineStatus.PlayingVoice:
                    path = "play.png";
                    break;
                case LineStatus.RecordingVoice:
                    path = "record.png";
                    break;
                case LineStatus.Dialing:
                    path = "dial.png";
                    break;
                case LineStatus.Diverting:
                    path = "divert.png";
                    break;
            }
            return ImageSourceExtension.GetImageSource(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
