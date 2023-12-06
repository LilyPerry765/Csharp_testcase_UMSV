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

namespace Pendar.Ums.CompositeNodes.UserControls
{


    public partial class CodeStatusVoiceDialog : Window
    {
        public CodeStatusVoiceDialog()
        {
            InitializeComponent();
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
