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
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Folder;
using UMSV.Schema;
using UMSV;
using Enterprise;


namespace UMSV
{
    /// <summary>
    /// Interaction logic for MonitoringForm.xaml
    /// </summary>
    public partial class MonitorSipAccountsForm : UserControl, IFolderForm
    {
        public MonitorSipAccountsForm()
        {
            InitializeComponent();
        }

        Timer timer;

        static MonitorSipAccountsForm()
        {
            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
        }

        private void RequestAccounts(object state)
        {
            VoIPServiceClient_Plugin_UMSV.Default.RequestAccounts();
        }

        private void Stop()
        {
            try
            {
                timer.Dispose();
                timer = null;
            }
            catch
            {
            }
        }

        private void Start()
        {
            timer = new Timer(RequestAccounts, null, 0, -1);
        }

        void Default_OnResponseAccounts(List<SipAccount> accounts)
        {
            Dispatcher.Invoke(((Action)(() => {
                AccountsPanel.Children.Clear();
                foreach (var account in accounts)
                    AccountsPanel.Children.Add(new AccountUserControl(account));
            })));

            System.Windows.Forms.Application.DoEvents();
            
            if (timer != null)
                timer.Change(Config.Default.MonitoringRefreshRate * 1000, -1);
        }

        private void StopMonitor(object sender, RoutedEventArgs e)
        {
            if ((string)PlayMenu.Header == "شروع")
            {
                Start();
                PlayMenu.Header = "متوقف";
            }
            else
            {
                Stop();
                PlayMenu.Header = "شروع";
            }
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccounts += Default_OnResponseAccounts;
            Start();

            helper.Closing += delegate
            {
                VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccounts -= Default_OnResponseAccounts;
                Stop();
            };

            helper.UnSelected += delegate
            {
                Stop();
            };

            helper.Selected += delegate
            {
                Start();
            };
        }

        #endregion
    }
}
