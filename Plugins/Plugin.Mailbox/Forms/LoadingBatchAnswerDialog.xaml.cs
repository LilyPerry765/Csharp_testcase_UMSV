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
using UMSV;
using Enterprise;
using System.IO;
using System.Threading;

namespace Plugin.Mailbox
{
    public partial class LoadingBatchAnswerDialog : Window
    {
        public List<string> files { get; set; }

        public LoadingBatchAnswerDialog()
        {
            InitializeComponent();
        }

        public LoadingBatchAnswerDialog(List<string> files)
        {
            InitializeComponent();
            this.files = files;
            batchAnswerLoaderProgressBar.Minimum = 0;
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
            Thread thread = new Thread(new ThreadStart(LoadFiles));
            thread.Start();
        }

        private void LoadFiles()
        {
            Dispatcher.Invoke(new Action(() => startButton.IsEnabled = false));
            using (UmsDataContext dc = new UmsDataContext())
            {
                Dispatcher.Invoke(new Action(() => batchAnswerLoaderProgressBar.Maximum = files.Count + 1));                
                string file = string.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    file = files[i];
                    Dispatcher.Invoke(new Action(() => batchAnswerLoaderProgressBar.Value = i));
                    //example: D:\Voice\r312518.wav ===> 312518
                    int followUpCode = int.Parse(file.Substring(file.LastIndexOf("\\") + 2, file.LastIndexOf(".") - 2 - file.LastIndexOf("\\")));
                    MailboxMessage questionMessage = dc.MailboxMessages.FirstOrDefault(t => t.FollowupCode == followUpCode && t.Type == (int)MailboxMessageType.Ask);

                    if (questionMessage == null)
                        continue;

                    MailboxMessage answer = new MailboxMessage()
                    {
                        Data = FileToByteArray(files[i]),
                        //file name example: r351669 which 351669 is the followup code.
                        FollowupCode = followUpCode,
                        ReceiveTime = new Folder.FolderDataContext().GetDate().Value,
                        Sender = Folder.User.Current.Username,
                        Type = (byte)MailboxMessageType.Answer,
                        BoxNo = questionMessage.BoxNo,
                    };

                    dc.MailboxMessages.InsertOnSubmit(answer);
                    dc.MailboxMessages.DeleteOnSubmit(questionMessage);

                    dc.SubmitChanges();

                }
            }
            Dispatcher.Invoke(new Action(() => startButton.IsEnabled = true));
            Dispatcher.Invoke(new Action(() => Folder.MessageBox.ShowInfo("عملیات با موفقیت انجام شد")));
            Dispatcher.Invoke(new Action(() => this.DialogResult = true));
            Dispatcher.Invoke(new Action(() => this.Close()));
        }
    }
}
