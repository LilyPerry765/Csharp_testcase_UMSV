using System.Data.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Plugin.Mailbox.Controls
{
    public partial class MessageSelector : UserControl
    {
        public MessageSelector()
        {
            InitializeComponent();
        }

        public Binary Message
        {
            get
            {
                return (byte[])GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(Binary), typeof(MessageSelector), new FrameworkPropertyMetadata(OnPropertyChanged)
            {
                BindsTwoWayByDefault = true
            });

        private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var me = sender as MessageSelector;

            if (e.Property == MessageProperty)
            {
                if (e.NewValue == null)
                    me.soundControl.Voice = null;
                else
                    me.soundControl.Voice = (e.NewValue as Binary).ToArray();
            }
        }

        private void defaultButton_Click(object sender, RoutedEventArgs e)
        {
            soundControl.Voice = null;
        }

        private void soundControl_VoiceChanged(object sender, Folder.Audio.VoiceChangedEventArgs e)
        {
            if (soundControl.Voice == null || soundControl.Voice.Length == 0)
                Message = null;
            else
                Message = soundControl.Voice;
        }
    }
}
