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

namespace Plugin.Mailbox
{


    public partial class AnswerDialog : Window
    {
        public AnswerDialog(MailboxMessage message)
        {
            InitializeComponent();
            this.Message = message;
        }

        public MailboxMessage Message;

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    MailboxMessage answer = new MailboxMessage()
                    {
                        Data = new System.Data.Linq.Binary(soundControl.Voice),
                        FollowupCode = Message.FollowupCode,
                        ReceiveTime = new Folder.FolderDataContext().GetDate().Value,
                        Sender = Folder.User.Current.Username,
                        Type = (byte)MailboxMessageType.Answer,
                        BoxNo = Message.BoxNo,
                    };

                    int expireTime;
                    if (int.TryParse(Expire.Text, out expireTime))
                        answer.ExpireDate = answer.ReceiveTime.AddMinutes(expireTime);

                    dc.MailboxMessages.InsertOnSubmit(answer);
                    dc.SubmitChanges();

                    if (RemoveAsk.IsChecked == true)
                    {
                        var askMessage = dc.MailboxMessages.First(m => m.ID == Message.ID);
                        dc.MailboxMessages.DeleteOnSubmit(askMessage);
                    }

                    dc.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Folder.MessageBox.ShowError("خطا در ذخیره پاسخ، لطفا پارامترهای وارده را بررسی کرده و دوباره سعی کنید!");
                return;
            }

            DialogResult = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Expire.Text = UMSV.Model.Config.Default.AnswerExpireMinutes.ToString();
            FolloupCode.Text = string.Format("{0}", Message.FollowupCode);
            soundControl.Voice = new byte[] { };
        }
    }
}
