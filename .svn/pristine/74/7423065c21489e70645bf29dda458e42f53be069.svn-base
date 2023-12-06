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
using System.IO;
using System.Threading;
using UMSV;
using NAudio.Wave;
using Folder.Audio;
using System.Windows.Forms;

namespace Plugin.Mailbox
{
    public partial class ExtractMessageDialog : Window
    {
        public List<MailboxMessage> messages { get; set; }

        //public ExtractMessageDialog()
        //{
        //    InitializeComponent();
        //}

        public ExtractMessageDialog(List<MailboxMessage> messages)
        {
            InitializeComponent();
            this.messages = messages;
            messageExtractProgressBar.Minimum = 0;
        }

        public static byte[] FileToByteArray(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, System.Convert.ToInt32(fs.Length));
            fs.Close();
            return data;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(ExtractFiles));
            thread.Start();
        }

        private void ExtractFiles()
        {
            Dispatcher.Invoke(new Action(() => startButton.IsEnabled = false));
            Dispatcher.Invoke(new Action(() => messageExtractProgressBar.Maximum = messages.Count + 1));

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            WaveStream waveStream;

            System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.Cancel;
            Dispatcher.Invoke(new Action(() => result = dialog.ShowDialog()));

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                int index = 0;
                foreach (MailboxMessage message in messages)
                {
                    Dispatcher.Invoke(new Action(() => messageExtractProgressBar.Value = index));

                    MemoryStream stream = new MemoryStream(message.Data.ToArray());
                    waveStream = new RawSourceWaveStream(stream, AudioUtility.AlawFormat);
                    waveStream = new WaveFormatConversionStream(AudioUtility.PcmFormat, waveStream);
                    waveStream.Position = 0;

                    string file = string.Format("{0}\\{1}_{2}{3}", dialog.SelectedPath, message.BoxNo, (index + 1).ToString(), ".wav");

                    var alawWaveStream = new WaveFormatConversionStream(AudioUtility.AlawFormat, waveStream);
                    WaveFileWriter.CreateWaveFile(file, alawWaveStream);

                    //var alawWaveStream = new WaveFormatConversionStream(AudioUtility.AlawFormat, waveStream);
                    //var stream = new RawSourceWaveStream(alawWaveStream, AudioUtility.AlawFormat);
                    //MemoryStream memstream = new MemoryStream();
                    //stream.CopyTo(memstream);
                    //System.IO.File.WriteAllBytes(string.Format("{0}/{1}-{2}-{3}{4}", dialog.SelectedPath, message.BoxNo, message.ReceiveTime.ToString()
                    //                                                            , (index + 1).ToString(), ".wav")
                    //                                                            , memstream.ToArray());

                    //WaveFileWriter.CreateWaveFile(string.Format("{0}/{1}-{2}-{3}{4}", dialog.SelectedPath, message.BoxNo, message.ReceiveTime.ToString()
                    //                                                            , (index + 1).ToString(), ".wav"), waveStream);

                    index++;
                }
            }

            Dispatcher.Invoke(new Action(() => startButton.IsEnabled = true));
            Dispatcher.Invoke(new Action(() => Folder.MessageBox.ShowInfo("عملیات با موفقیت انجام شد")));
            Dispatcher.Invoke(new Action(() => this.DialogResult = true));
            Dispatcher.Invoke(new Action(() => this.Close()));
        }
    }
}
