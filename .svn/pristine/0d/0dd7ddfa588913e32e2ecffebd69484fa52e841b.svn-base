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
using System.Net;
using UMSV.Schema;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for OperatorsMonitoringForm.xaml
    /// </summary>
    public partial class OperatorsMonitoringForm : UserControl, IFolderForm
    {
        public OperatorsMonitoringForm()
        {
            InitializeComponent();
        }

        static OperatorsMonitoringForm()
        {
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
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            RequestInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<Schema.SipAccount> accounts, List<SipDialog> dialogs)
        {
            var users = User.Current.SubUsers.Where(u => accounts.Any(a => a.UserID == u.Username)).OrderBy(u => u.Username);
            var operators = new List<OperatorSipStatus>();
            foreach (var user in users)
            {
                var account = accounts.First(a => a.UserID == user.Username);
                var opr = new OperatorSipStatus()
                {
                    Username = user.Username,
                    Fullname = user.Fullname,
                    Group = user.RoleNames,
                    EndPoint = account.SipEndPoint,
                    Status = account.Status,
                    RegisterTime = account.RegisterTime,
                };

                if (account.Status == SipAccountStatus.Talking || account.Status == SipAccountStatus.Dialing || account.Status == SipAccountStatus.Hold)
                {
                    var dialog = dialogs.FirstOrDefault(d => d.Status == DialogStatus.Talking && d.CalleeID == user.Username);
                    if (dialog != null)
                    {
                        opr.CallerID = dialog.CallerID;
                        opr.CallTime = dialog.CallTime;
                    }
                }

                operators.Add(opr);
            }

            Dispatcher.Invoke(((Action)(() =>
            {
                Panel.Children.Clear();
                foreach (var group in operators.Select(u => u.Group).Distinct())
                {
                    var groupBox = new GroupBox()
                    {
                        Header = group
                    };
                    Panel.Children.Add(groupBox);
                    groupBox.Content = new WrapPanel();

                    foreach (var opr in operators.Where(o => o.Group == group))
                    {
                        OperatorStatusControl control = new OperatorStatusControl();
                        string title = string.Format("Username: {0}\r\nFullname: {1}\r\nStatus: {2}", opr.Username, opr.Fullname, opr.Status);

                        switch (opr.Status)
                        {
                            case SipAccountStatus.Offline:
                                control.Opacity = .35;
                                break;

                            case SipAccountStatus.DND:
                                control.Opacity = .35;
                                control.DndImage.Visibility = System.Windows.Visibility.Visible;
                                break;

                            case SipAccountStatus.Dialing:
                            case SipAccountStatus.Talking:
                            case SipAccountStatus.Hold:
                                title += string.Format("\r\nCallerID: {0}", opr.CallerID);
                                if (opr.CallTime.HasValue)
                                {
                                    title += string.Format("\r\nCallTime: {0}", opr.CallTime.Value.TimeOfDay);
                                    control.TalkingImage.Visibility = System.Windows.Visibility.Visible;
                                }
                                break;
                        }

                        if (opr.Status != SipAccountStatus.Offline)
                            title += string.Format("\r\nEndPoint: {0}", opr.EndPoint);

                        if (opr.RegisterTime.HasValue)
                            title += string.Format("\r\nRegisterTime: {0}", opr.RegisterTime.Value.TimeOfDay);

                        control.ToolTip = title;
                        control.TitleBlock.Text = string.Format("{0}\r\n{1}", opr.Fullname, opr.Username);
                        (groupBox.Content as WrapPanel).Children.Add(control);
                    }
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