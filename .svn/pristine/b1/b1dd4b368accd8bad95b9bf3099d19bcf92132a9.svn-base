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
using UMSV.Schema;
using Folder;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for AccountUserControl.xaml
    /// </summary>
    public partial class AccountUserControl : UserControl
    {
        public AccountUserControl(SipAccount account)
        {
            InitializeComponent();
            using (Folder.FolderDataContext dc = new FolderDataContext())
            {
                User user = dc.Users.FirstOrDefault(p => p.Username == account.UserID);
                if (user != null && !String.IsNullOrEmpty(user.Fullname))
                {
                    TitleBlock.Content = string.Format("{0} ({1})", account.UserID, user.Fullname);
                    ToolTip = string.Format("User: {0}\r\nName: {1}\r\nRegisterTime: {2}\r\nExpireTime: {3}\r\nLast Call Time: {4}\r\nEndPoint: {5}",
                        account.UserID,
                        user.Fullname,
                        account.RegisterTime.HasValue ? new PersianDateTime(account.RegisterTime.Value).ToString("HH:mm") : "-",
                        account.ExpireTime.HasValue ? new PersianDateTime(account.ExpireTime.Value).ToString("HH:mm") : "-",
                        account.LastCallTime != DateTime.MinValue ? new PersianDateTime(account.LastCallTime).ToString("HH:mm") : "-",
                        account.SipEndPoint
                        );
                    Opacity = account.Status != SipAccountStatus.Offline ? 1 : .3;
                }
                else
                {
                    TitleBlock.Content = string.Format("{0} ({1})", account.UserID, account.DisplayName);
                    ToolTip = string.Format("User: {0}\r\nName: {1}\r\nRegisterTime: {2}\r\nExpireTime: {3}\r\nLast Call Time: {4}\r\nEndPoint: {5}",
                        account.UserID,
                        account.DisplayName,
                        account.RegisterTime.HasValue ? new PersianDateTime(account.RegisterTime.Value).ToString("HH:mm") : "-",
                        account.ExpireTime.HasValue ? new PersianDateTime(account.ExpireTime.Value).ToString("HH:mm") : "-",
                        account.LastCallTime != DateTime.MinValue ? new PersianDateTime(account.LastCallTime).ToString("HH:mm") : "-",
                        account.SipEndPoint
                        );
                    Opacity = account.Status != SipAccountStatus.Offline ? 1 : .3;
                }

            }
        }
    }
}
