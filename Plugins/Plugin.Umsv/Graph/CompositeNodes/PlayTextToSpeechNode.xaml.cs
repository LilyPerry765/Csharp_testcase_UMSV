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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using UMSV.ViewModels;

namespace Pendar.Ums.CompositeNodes
{
    /// <summary>
    /// Interaction logic for PlayTextToSpeechNode.xaml
    /// </summary>
    [UMSV.CompositeNode(Tag = "PlayText", Icon = "images/TTS.png", Title = "خواندن متن", GroupIndex = -1, Index = 0)]
    public partial class PlayTextToSpeechNode : UserControl
    {
        public PlayTextToSpeechNode()
        {
            InitializeComponent();
        }

        private void playBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(messageTxt.Text))
            {
                System.Speech.Synthesis.SpeechSynthesizer speech = new System.Speech.Synthesis.SpeechSynthesizer();
                var format = new SpeechAudioFormatInfo(EncodingFormat.ALaw, 8000, 8, 1, 8000, 1, null);
                speech.SetOutputToDefaultAudioDevice();

                PromptBuilder pbuilder = new PromptBuilder();
                PromptStyle pStyle = new PromptStyle();

                pStyle.Emphasis = PromptEmphasis.NotSet;
                pStyle.Rate = PromptRate.Slow;
                pStyle.Volume = PromptVolume.ExtraLoud;

                pbuilder.StartStyle(pStyle);
                pbuilder.StartVoice(VoiceGender.Female, VoiceAge.Teen, 2);
                pbuilder.StartSentence();
                pbuilder.AppendText(messageTxt.Text.Trim('[', ']').Trim('\"'));
                pbuilder.EndSentence();
                pbuilder.EndVoice();
                pbuilder.EndStyle();

                speech.Speak(pbuilder);
            }
        }

        private CompositeNodeViewModel ViewModel
        {
            get
            {
                return this.DataContext as CompositeNodeViewModel;
            }
        }

        private void messageTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(messageTxt.Text))
            {
                messageTxt.Text = messageTxt.Text.Trim('[', ']');
                messageTxt.Text = messageTxt.Text.Trim('\"');
                messageTxt.Text = String.Format("[\"{0}\"]", messageTxt.Text);
                ViewModel.NodeData.PlayNodes[0].Voice.Clear();
                ViewModel.NodeData.PlayNodes[0].Voice.Add(new UMSV.Schema.Voice() { Name = messageTxt.Text });
            }


        }
    }
}
