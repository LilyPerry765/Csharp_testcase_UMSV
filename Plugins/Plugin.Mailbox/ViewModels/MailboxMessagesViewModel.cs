using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder.Commands;
using UMSV;

namespace Plugin.Mailbox.ViewModels
{
    public class MailboxMessagesViewModel : DataDrivenViewModel
    {
        private string boxNo = null;

        public MailboxMessagesViewModel()
        {
        }

        public MailboxMessagesViewModel(string boxNo)
        {
            this.boxNo = boxNo;
        }

        public IEnumerable<MailboxMessage> Messages
        {
            get
            {
                if (boxNo == null)
                    return DB.MailboxMessages;
                else
                    return DB.MailboxMessages.Where(m => m.BoxNo == boxNo);
            }
        }

        private DelegateCommand _RefreshCommand;
        public DelegateCommand RefreshCommand
        {
            get
            {
                if (_RefreshCommand == null)
                {
                    _RefreshCommand = new DelegateCommand(delegate
                    {
                        SendPropertyChanged("Messages");
                    });
                }
                return _RefreshCommand;
            }
        }
    }
}
