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
using System.Threading;
using Enterprise;

namespace UMSV.Forms
{
    /// <summary>
    /// Interaction logic for DashboardPanel.xaml
    /// </summary>
    public partial class DashboardPanel : UserControl, IFolderForm
    {
        public DashboardPanel()
        {
            InitializeComponent();
            OperatorsGauge.MinValue = 0;
            OperatorsGauge.MaxValue = UMSV.Schema.Config.Default.ActivePhoneLines;
            OperatorsGauge.OptimalRangeStartValue = OperatorsGauge.MaxValue / 2;
            OperatorsGauge.OptimalRangeEndValue = OperatorsGauge.MaxValue * 3 / 4;
        }

        private void RequestInfoFromServer(object state)
        {
            try
            {
                Logger.WriteView("RequestInfoFromServer ...");
                VoipServiceClient.Default.RequestAccountsAndDialogs();
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            finally
            {
                if (requestInfoFromServerTimer != null)
                    requestInfoFromServerTimer.Change(60 * 1000, -1);
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


            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
            
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            VoipServiceClient.Default.OnResponseAccountsAndDialogs += Default_OnResponseAccountsAndDialogs;

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
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            RequestInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<Schema.SipAccount> accounts, List<SipDialog> dialogs, int workingPartirionSize, int workingPartirionFreeSize)
        {
            Dispatcher.Invoke((Action)(() => {
                try
                {
                    var oprs = accounts.Where(a => a.MaxConcurrentCalls == 1 && a.Status != Schema.SipAccountStatus.Offline);

                    OperatorsButton.Text = oprs.Count().ToString();
                    int busyCount = oprs.Where(a => a.Status != Schema.SipAccountStatus.Online).Count();
                    SubscriberButton.Text = (dialogs.Count - busyCount).ToString();
                    BusySubscriberButton.Text = (busyCount).ToString();

                    OperatorsGauge.CurrentValue = (dialogs.Count - busyCount);
                    LastUpdateLabel.Text = serverTime.ToString("HH:mm:ss");
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }));

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            VoipServiceClient.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
            Stop();
        }

        #endregion
    }
}
