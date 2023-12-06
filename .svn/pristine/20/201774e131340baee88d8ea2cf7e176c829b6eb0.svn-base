using System.Linq;
using System;
using Folder.Commands;
using Enterprise;
using UMSV;


namespace Plugin.Mailbox.ViewModels
{
    public partial class MailboxBatchViewModel : DataDrivenViewModel
    {
        private bool IsCancelled;

        public MailboxBatchViewModel()
        {
            ModelMailbox = new BatchMailbox();
            ProgressValue = 0;
            ProgressMax = 100;
            SendPropertyChanged();
        }

        protected override bool OnSave()
        {
            InsertMailBoxes();
            DialogResult = true;
            SendPropertyChanged();
            return true;
        }

        private void InsertMailBoxes()
        {
            Logger.WriteInfo("Batch creation of Mail Box started...");

            var boxNos = DB.Mailboxes.Select(b => b.BoxNo).ToArray();
            var m = ModelMailbox;
            var rnd = new Random();

            ProgressMax = ModelMailbox.BoxNoTo - ModelMailbox.BoxNoFrom;

            for (int i = ModelMailbox.BoxNoFrom; i <= ModelMailbox.BoxNoTo; i++)
            {
                if (IsCancelled)
                {
                    Logger.WriteWarning("Batch creation of Mail Box cancelled by user.");
                    return;
                }

                ProgressValue = i;
                SendPropertyChanged();
                System.Windows.Forms.Application.DoEvents();
                if (!boxNos.Any(b => b == i.ToString()))
                {
                    var newMailBox = new UMSV.Mailbox()
                    {
                        ActivationDate = m.ActivationDate,
                        AutoDequeueFullMailbox = m.AutoDequeueFullMailbox,
                        BoxNo = i.ToString(),
                        Comment = m.Comment,
                        ExpirationDate = m.ExpirationDate,
                        MaxNewMessage = m.MaxNewMessage,
                        MaxArchiveMessage = m.MaxArchiveMessage,
                        MessageNewExpirePeriod = m.MessageNewExpirePeriod,
                        MessageAnswerExpirePeriod = m.MessageAnswerExpirePeriod,
                        MessageAskExpirePeriod = m.MessageAskExpirePeriod,
                        AccessId = Guid.NewGuid()
                    };

                    if (m.BoxNoAsCallerID)
                        newMailBox.CallerID = i.ToString();

                    if (m.RandomPassword)
                        newMailBox.Password = rnd.Next(1000, int.MaxValue).ToString();

                    DB.Mailboxes.InsertOnSubmit(newMailBox);
                    DB.SubmitChanges();
                }
            }
            Logger.WriteInfo("Batch creation of Mail Box finished.");

        }

        public double ProgressMax
        {
            get;
            set;
        }

        public double ProgressValue
        {
            get;
            set;
        }

        public BatchMailbox ModelMailbox
        {
            get;
            private set;
        }

        private DelegateCommand _CancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                {
                    _CancelCommand = new DelegateCommand(delegate
                    {
                        IsCancelled = true;
                        DialogResult = false;
                    });
                }
                return _CancelCommand;
            }
        }


    }
}
