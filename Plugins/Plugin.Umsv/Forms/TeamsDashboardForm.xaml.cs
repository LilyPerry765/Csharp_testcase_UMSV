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
using System.Threading;
using Folder;
using Enterprise;
using System.Net;
using UMSV.Schema;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for TeamsDashboardForm.xaml
    /// </summary>
    public partial class TeamsDashboardForm : UserControl, IFolderForm
    {
        public TeamsDashboardForm ()
        {
            InitializeComponent();
        }

        static TeamsDashboardForm()
        {
            OperatorTeamsMonitoringForm.ReloadTeams();
            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
        }

        private void RequestInfoFromServer(object state)
        {
            try
            {
                //Logger.WriteView("RequestInfoFromServer ...");
                VoIPServiceClient_Plugin_UMSV.Default.RequestAccountsAndDialogs();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        private Timer requestInfoFromServerTimer;

        private void Stop()
        {
            try
            {
                requestInfoFromServerTimer.Dispose();
                requestInfoFromServerTimer = null;
            }
            catch
            {
            }
        }

        private void Start()
        {
            requestInfoFromServerTimer = new Timer(RequestInfoFromServer, null, 0, -1);
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccountsAndDialogs += Default_OnResponseAccountsAndDialogs;

            Start();

            helper.Closing += new EventHandler<FormClosingEventArgs>(helper_Closing);
            helper.UnSelected += delegate
            {
                Stop();
            };

            helper.Selected += delegate
            {
                Start();
            };
            helper.Refresh += new EventHandler<RefreshEventArgs>(helper_Refresh);

            CreateControls();
        }

        private void CreateControls()
        {
            foreach (var team in OperatorTeamsMonitoringForm.Teams.OrderBy(t => t.TeamRole.Name))
            {
                var item = new TeamsDashboardItem();
                item.DataContext = team;
                Panel.Children.Add(item);
            }
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            RequestInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<Schema.SipAccount> accounts, List<SipDialog> dialogs)
        {
            Dispatcher.Invoke(((Action)(() =>
            {
                foreach (TeamsDashboardItem groupBox in Panel.Children)
                {
                    groupBox.Refresh(dialogs, accounts);
                }
            })));

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
            Stop();
        }

        #endregion
    }
}
