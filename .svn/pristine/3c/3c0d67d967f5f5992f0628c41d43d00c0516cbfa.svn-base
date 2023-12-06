using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Folder.Audio;

namespace Plugin.Mailbox
{


    public partial class EditVoiceDialog : Window
    {
        public EditVoiceDialog()
        {
            InitializeComponent();
        }

        public bool MessagePlayed
        {
            get
            {
                return soundControl.VoicePlayed;
            }
        }

        public byte[] VoiceData
        {
            get
            {
                return soundControl.Voice;
            }
            set
            {
                soundControl.Voice = value;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
