using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using Enterprise;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UMSV
{
    public class Global : PluginGlobal
    {
        public Image TalkingImage;
        public static Global Default;

        public Global()
        {
            Default = this;
        }

        public override void SessionEnd()
        {
            Folder.Console.RemoveStatusBarElement(TalkingImage);
        }

        public override void SessionStart()
        {
            TalkingImage = new Image() {
                Width = 24,
                Height = 24,
                Visibility = System.Windows.Visibility.Hidden,
                Source = new BitmapImage(new Uri(Constants.TalkingImage, UriKind.RelativeOrAbsolute)),
            };
            Folder.Console.AddStatusBarElement(TalkingImage);
        }
    }
}

