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
    public partial class MonitorSipDialogsForm : UserControl, IFolderForm
    {
        public MonitorSipDialogsForm()
        {
            InitializeComponent();
        }

        Timer timer;

        static MonitorSipDialogsForm()
        {
            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
        }

        private void RequestDialogs(object state)
        {
            VoIPServiceClient_Plugin_UMSV.Default.RequestDialogs();
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
            timer = new Timer(RequestDialogs, null, 0, -1);
        }

        void Default_OnResponseDialogs(List<SipDialog> dialogs)
        {
            Dispatcher.Invoke(((Action)(() => {
                DialogsPanel.Children.Clear();
                foreach (var dialog in dialogs)
                    DialogsPanel.Children.Add(new DialogUserControl(dialog));
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
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseDialogs += Default_OnResponseDialogs;
            Start();

            helper.Closing += delegate
            {
                VoIPServiceClient_Plugin_UMSV.Default.OnResponseDialogs -= Default_OnResponseDialogs;
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
