using System.Windows.Markup;
using System.Windows;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media;
using System.Windows.Controls;
using System.Reflection;


namespace Plugin.Mailbox.Assets
{
    public class ImageSourceExtension : MarkupExtension
    {
        public string Path
        {
            get;
            set;
        }

        public ImageSourceExtension()
            : base()
        {
        }

        public ImageSourceExtension(string path)
            : this()
        {
            Path = path;
        }


        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            var source = new BitmapImage(new Uri(FindPath(Path)));
            var prop = target.TargetProperty as DependencyProperty;

            if (prop == null || prop.PropertyType != typeof(ImageSource))
            {
                var img = new Image();
                if (Width != 0) img.Width = Width;
                if (Height != 0) img.Height = Height;
                img.Source = source;
                return img;
            }

            return source;
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }

        private static string FindPath(string path)
        {
            string asmName = Assembly.GetExecutingAssembly().GetName().Name;
            string uriString = string.Format("pack://application:,,,/{0};component/Images/{1}", asmName, path);
            return uriString;
        }

        public static ImageSource GetImageSource(string path)
        {
            ImageSource imgSrc = new BitmapImage(new Uri(FindPath(path)));
            return imgSrc;
        }

    }
}
