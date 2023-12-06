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
using Folder;
using UMSV;
using System.IO;
using NAudio.Wave;
using Folder.Audio;
using Enterprise;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace Plugin.Mailbox
{
    public partial class MailboxMessagesForm : UserControl, IDataGridForm, IEditableForm, IFolderForm
    {
        UMSV.UmsDataContext dc = new UMSV.UmsDataContext();
        public string BoxNo;
        List<MailboxMessage> Messages = new List<MailboxMessage>();

        public MailboxMessagesForm()
        {
            InitializeComponent();
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new EditVoiceDialog();
            var message = dataGrid.CurrentCellItem<UMSV.MailboxMessage>();
            if (message.Data != null)
                dialog.VoiceData = message.Data.ToArray();

            if (dialog.ShowDialog() == true)
                message.Data = new System.Data.Linq.Binary(dialog.VoiceData);

            if (dialog.MessagePlayed)
            {
                UMSV.Mailbox mailbox = dc.Mailboxes.Single(t => t.BoxNo == message.BoxNo);
                if (mailbox.AutoArchive.HasValue && mailbox.AutoArchive.Value)
                    message.Type = (byte)MailboxMessageType.Archive;
            }
        }

        public DataGrid DataGrid
        {
            get { return dataGrid; }
        }

        public bool Save()
        {
            dc.SubmitChanges();
            return true;
        }

        public void Initialize(FolderFormHelper helper)
        {
            helper.Refresh += new EventHandler<RefreshEventArgs>(helper_Refresh);
            if (!string.IsNullOrEmpty(BoxNo))
            {
                BoxNoTextbox.Text = BoxNo.ToString();
                BoxNoTextbox.IsEnabled = false;
            }

            var items = Folder.Utility.CreateEnumDataSource<byte>(typeof(MailboxMessageType));
            items.Insert(0, new NameValue());
            TypeComboxBox.ItemsSource = items;
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!string.IsNullOrEmpty(BoxNo))
                dataGrid.IsReadOnly = BoxNo != User.Current.Username && (MailboxType)dc.Mailboxes.First(m => m.BoxNo == BoxNo).Type == MailboxType.Private;
            else
                dataGrid.IsReadOnly = true;

            //foreach (var mailbox in folder.Mailboxes)
            //{
            //    CheckAccess(mailbox);
            //    //graph.PropertyChanged += GraphsListViewModel_PropertyChanged;
            //}

            //var mailboxNumbers = folder.Mailboxes.ToList().Where(t => Folder.User.IsInRole(t.AccessId)).Select(t => t.BoxNo);
            //var messages = folder.MailboxMessages.Where(t => mailboxNumbers.Contains(t.BoxNo));

            List<string> accessibleMailboxNumbers = AccessibleMailboxNumbers(Folder.User.Current.AllRoles);
            var messages = accessibleMailboxNumbers.Count > 2100 ? dc.MailboxMessages :
                                                                   dc.MailboxMessages.Where(t => accessibleMailboxNumbers.Contains(t.BoxNo));


            messages = messages.Where(m => string.IsNullOrWhiteSpace(BoxNoTextbox.Text) || m.BoxNo == BoxNoTextbox.Text);

            if (!string.IsNullOrWhiteSpace(SenderTextbox.Text))
                messages = messages.Where(m => m.Sender == SenderTextbox.Text);

            if (!string.IsNullOrWhiteSpace(FollowupCodeTextbox.Text))
                messages = messages.Where(m => m.FollowupCode.HasValue && m.FollowupCode.Value.ToString() == FollowupCodeTextbox.Text);

            if (!string.IsNullOrWhiteSpace(CommentTextbox.Text))
                messages = messages.Where(m => m.Comment.IndexOf(CommentTextbox.Text) > -1);

            if (TypeComboxBox.SelectedIndex > 0)
                messages = messages.Where(m => m.Type == (byte)TypeComboxBox.SelectedValue);

            if (FromDate.SelectedDate.HasValue)
                messages = messages.Where(m => m.ReceiveTime >= FromDate.SelectedDate.Value);

            if (ToDate.SelectedDate.HasValue)
                messages = messages.Where(m => m.ReceiveTime <= ToDate.SelectedDate.Value);

            if (ExpiredMessage.IsChecked == true)
            {
                var now = new Folder.FolderDataContext().GetDate().Value;
                messages = messages.Where(m => m.ExpireDate.HasValue && m.ExpireDate.Value < now);
            }

            Messages = messages.ToList();
            dataGrid.ItemsSource = Messages;
            //<Label Name="PlayFromPublicAlert" Visibility="Collapsed" Content="توجه: تنها پیامهای نوع عمومی در گره پخش از صندوق صوتی عمومی استفاده خواهند شد." Foreground="Red" />
            //PlayFromPublicAlert.Visibility = dataGrid.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void dataGrid_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            EditMenu.Visibility = ConvertToPublic.Visibility = ConvertToNew.Visibility = System.Windows.Visibility.Collapsed;
            var message = dataGrid.CurrentCellItem<UMSV.MailboxMessage>();
            if (message == null)
                return;

            EditMenu.Visibility = System.Windows.Visibility.Visible;

            if (message.BoxNo == User.Current.Username || (message.Mailbox != null && (UMSV.MailboxType)message.Mailbox.Type == UMSV.MailboxType.Public))
            {
                ConvertToNew.Visibility = (message.Type == (byte)MailboxMessageType.PublicMessage) ? Visibility.Visible : Visibility.Collapsed;
                ConvertToPublic.Visibility = (message.Type != (byte)MailboxMessageType.PublicMessage) ? Visibility.Visible : Visibility.Collapsed;
            }

            AnswerMenu.Visibility = message.Type == (byte)MailboxMessageType.Ask ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            var newMessage = new MailboxMessage()
            {
                BoxNo = BoxNo,
                ReceiveTime = DateTime.Now,
                Sender = User.Current.Username,
                Type = (byte)MailboxMessageType.New
            };
            dc.MailboxMessages.InsertOnSubmit(newMessage);
            Messages.Add(newMessage);
            dataGrid.Items.Refresh();
        }

        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            var message = (e.NewItem as MailboxMessage);
            message.BoxNo = BoxNo;
            message.ReceiveTime = DateTime.Now;
            message.Sender = User.Current.Username;
            message.Type = (byte)MailboxMessageType.New;
            message.Data = new System.Data.Linq.Binary(new byte[] { });
        }

        private void ConvertToPublic_Click(object sender, RoutedEventArgs e)
        {
            var message = dataGrid.CurrentCellItem<MailboxMessage>();
            message.Type = (byte)MailboxMessageType.PublicMessage;
        }

        private void ConvertToNew_Click(object sender, RoutedEventArgs e)
        {
            var message = dataGrid.CurrentCellItem<MailboxMessage>();
            message.Type = (byte)MailboxMessageType.New;
        }

        private void ImportMenu_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog()
            {
                Multiselect = true,
                Title = "لطفا فایلهای صوتی مورد نظر را انتخاب کنید.",
                Filter = "All Supported Audio Files (*.wav;*.mp3;*.raw)|*.wav;*.mp3;*.raw|All Files (*.*)|*.*",
            };
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            int index = 0;
            foreach (var file in dialog.FileNames)
            {
                try
                {
                    var fileStream = new MemoryStream(System.IO.File.ReadAllBytes(file));
                    WaveStream waveStream;

                    switch (System.IO.Path.GetExtension(file).ToLower())
                    {
                        case ".wave":
                        case ".wav":
                            waveStream = new WaveFileReader(fileStream);
                            break;

                        case ".mp3":
                            waveStream = new Mp3FileReader(fileStream);
                            break;

                        case ".raw":
                            waveStream = new RawSourceWaveStream(fileStream, AudioUtility.AlawFormat);
                            break;

                        default:
                            continue;
                    }
                    waveStream = new WaveFormatConversionStream(AudioUtility.PcmFormat, waveStream);
                    waveStream.Position = 0;
                    var alawStream = new WaveFormatConversionStream(AudioUtility.AlawFormat, waveStream);
                    var raw = new RawSourceWaveStream(alawStream, AudioUtility.AlawFormat);

                    MemoryStream stream = new MemoryStream();
                    raw.Position = 0;
                    raw.CopyTo(stream);

                    MailboxMessage message = new MailboxMessage()
                    {
                        BoxNo = BoxNo,
                        Comment = file,
                        Type = (byte)MailboxMessageType.PublicMessage,
                        Sender = User.Current.Username,
                        ReceiveTime = DateTime.Now,
                        Data = stream.ToArray(),
                    };
                    dc.MailboxMessages.InsertOnSubmit(message);
                    index++;
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }

            dc.SubmitChanges();
            Refresh();

            Folder.MessageBox.ShowInfo("{0} فایل با موفقیت وارد شد.", index);
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dataGrid.SelectedItems.Count > 0)
            {
                foreach (var item in dataGrid.SelectedItems)
                {
                    Messages.Remove((MailboxMessage)item);
                    dc.MailboxMessages.DeleteOnSubmit((MailboxMessage)item);
                }
                dataGrid.Items.Refresh();
            }
        }

        private void AnswerItem_Click(object sender, RoutedEventArgs e)
        {
            var message = dataGrid.CurrentCellItem<UMSV.MailboxMessage>();
            AnswerDialog dialog = new AnswerDialog(message);
            if (dialog.ShowDialog() == true)
                Refresh();
        }

        private void ArchiveMessage_Click(object sender, RoutedEventArgs e)
        {
            var message = dataGrid.CurrentCellItem<MailboxMessage>();
            message.Type = (byte)MailboxMessageType.Archive;
        }

        private void ExtractMessage_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItems != null && dataGrid.SelectedItems.Count > 0)
            {
                List<MailboxMessage> messages = dataGrid.SelectedItems.OfType<MailboxMessage>().ToList();
                ExtractMessageDialog dialog = new ExtractMessageDialog(messages);
                dialog.ShowDialog();
            }
        }

        private void ImportAnswerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] attachments = new string[0];
                List<string> attachmentsToLoad = new List<string>();
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Multiselect = true;
                dialog.Filter = "wav files (*.wav)|*.wav";
                bool? res = dialog.ShowDialog();

                if (res.HasValue && res.Value)
                {
                    attachments = dialog.FileNames;
                    int allFilesCount = attachments.Length;
                    int wrongFilesCount = 0;
                    if (attachments.Length == 0)
                    {
                        Folder.MessageBox.ShowWarning("فایلی انتخاب نشد");
                        return;
                    }
                    else
                    {
                        foreach (string attachment in attachments)
                        {
                            if (attachment.LastIndexOf(".") == -1 ||
                                attachment.Substring(attachment.LastIndexOf(".")).ToLower() != ".wav" ||
                                !Regex.IsMatch(attachment.Substring(attachment.LastIndexOf("\\") + 1), @"\br\d{1,}\b.wav"))
                            {
                                wrongFilesCount++;
                                continue;
                            }

                            attachmentsToLoad.Add(attachment);
                        }
                    }

                    if (Folder.MessageBox.ShowQuestion(string.Format("از {0} فایل انتخاب شده، {1} فایل مجاز می باشد. آیا از ذخیره پاسخ ها در سیستم اطمینان دارید؟", allFilesCount, allFilesCount - wrongFilesCount), "پرسش") != MessageBoxResult.Yes)
                        return;


                    //AnswerDialog d = new AnswerDialog(new MailboxMessage());
                    //d.ShowDialog();
                    LoadingBatchAnswerDialog loadingBatchAnswerDialog = new LoadingBatchAnswerDialog(attachmentsToLoad);
                    loadingBatchAnswerDialog.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Folder.MessageBox.ShowError("خطا در شناسایی فایل ها");
                Logger.Write(ex);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private List<string> AccessibleMailboxNumbers(List<Role> currentUserRoles)
        {
            using (UmsDataContext context = new UmsDataContext())
            {
                if (User.Current.AllRoles.Select(t => t.ID).Contains(UMSV.Constants.Role_MailboxAccess))
                    return context.Mailboxes.Select(t => t.BoxNo)
                        .ToList();

                return context.Mailboxes.Where(t => User.Current.AllRoles.Select(c => c.ID).Contains(t.AccessId))
                    .Select(t => t.BoxNo)
                    .ToList();
            }
        }

    }
}
