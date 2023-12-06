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
using Enterprise;
using Folder;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for AccountUserControl.xaml
    /// </summary>
    public partial class DialogUserControl : UserControl
    {
        public DialogUserControl(SipDialog dialog)
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(dialog.DialogID) || dialog.CallTime == DateTime.MinValue)
                Logger.WriteWarning("Invalid Dialog CallID is null, or datetime is invalid.");
            else
                this.ToolTip = TitleBlock.Content = string.Format("CallerID: {0}\r\nCalleeID: {1}\r\nDialogID: {2}\r\nStartTime: {3}\r\nStatus: {4}",
                         dialog.CallerID, dialog.CalleeID, dialog.DialogID, new PersianDateTime(dialog.CallTime).ToString("HH:mm"), dialog.Status);
        }
    }
}
