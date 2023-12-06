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
using Folder;
using UMSV.Schema;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for TeamsDashboardItem.xaml
    /// </summary>
    public partial class TeamsDashboardItem : UserControl
    {
        public TeamsDashboardItem()
        {
            InitializeComponent();
        }

        internal void Refresh(List<SipDialog> dialogs, List<Schema.SipAccount> accounts)
        {
            QueuedCalls.Value = dialogs.Count(d => !string.IsNullOrEmpty(d.DivertTargetTeam) && Guid.Parse(d.DivertTargetTeam) == Team.TeamRole.ID && d.Status == DialogStatus.WaitForDiverting);

            var teamAccounts = accounts.Where(a => Team.Members.Select(u => u.Username).Contains(a.UserID));

            TotalDialogs.Value = teamAccounts.Count(d => d.Status == SipAccountStatus.Talking || d.Status == SipAccountStatus.Hold);
            DndUsers.Value = teamAccounts.Count(d => d.Status == SipAccountStatus.DND);
            OnlineUsers.Value = teamAccounts.Count(d => d.Status != SipAccountStatus.Offline);
        }

        public OperatorTeam Team
        {
            get
            {
                return DataContext as OperatorTeam;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Panel.Header = Team.TeamRole.Name;
        }
    }
}
