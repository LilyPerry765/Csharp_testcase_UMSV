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
    public partial class OperatorTeamsMonitoringForm2 : UserControl, IFolderForm
    {
        public OperatorTeamsMonitoringForm2()
        {
            InitializeComponent();
        }


        private void RequestInfoFromServer(object state)
        {
            try
            {
                //Logger.WriteView("RequestInfoFromServer ...");
                VoipServiceClient.Default.RequestAccountsAndDialogs();
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

            CreateControls();
        }

        private void CreateControls()
        {
            foreach (var team in Teams.OrderBy(t => t.TeamRole.Name))
            {
                var groupBox = new Expander() {
                    Header = new TextBlock() {
                        Text = team.TeamRole.Name,
                        FontWeight = FontWeights.Bold,
                    },
                    Padding = new Thickness(5),
                    Tag = team.TeamRole,
                };

                Panel.Children.Add(groupBox);

                StackPanel panel = new StackPanel();
                groupBox.Content = panel;

                panel.Children.Add(new WrapPanel());
                panel.Children.Add(new Border() {
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(3),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Child = new StackPanel() {
                        Orientation = Orientation.Horizontal,
                    }
                });

                foreach (var opr in team.Members)
                {
                    OperatorStatusControl2 control = new OperatorStatusControl2();
                    control.Margin = new Thickness(2);
                    control.Tag = opr;
                    control.ToolTip = string.Format("Username: {0}\r\nFullname: {1}", opr.Username, opr.Fullname);
                    control.TitleBlock.Content = opr.Fullname;
                    (panel.Children[0] as WrapPanel).Children.Add(control);
                }
            }
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            RequestInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<Schema.SipAccount> accounts, List<SipDialog> dialogs, int workingPartirionSize, int workingPartirionFreeSize)
        {
            Dispatcher.Invoke(((Action)(() => {
                foreach (Expander groupBox in Panel.Children)
                {
                    var group = groupBox.Content as StackPanel;

                    var queuePanel = (group.Children[1] as Border).Child as StackPanel;
                    queuePanel.Children.Clear();
                    foreach (var dialog in dialogs.Where(d =>
                        !string.IsNullOrEmpty(d.DivertTargetTeam) &&
                        Guid.Parse(d.DivertTargetTeam) == (groupBox.Tag as Role).ID &&
                        d.Status == DialogStatus.WaitForDiverting))
                    {
                        queuePanel.Children.Add(new TextBox() {
                            Background = Brushes.Yellow,
                            Foreground = Brushes.Black,
                            BorderBrush = Brushes.Gray,
                            Text = dialog.CallerID,
                            ToolTip = string.Format("زمان تماس: {0}", dialog.CallTime.TimeOfDay),
                            Margin = new Thickness(3),
                        });
                    }

                    foreach (OperatorStatusControl2 control in (group.Children[0] as WrapPanel).Children)
                    {
                        var opr = control.Tag as User;
                        var account = accounts.FirstOrDefault(a => a.UserID == opr.Username);
                        var gatewayDialog = dialogs.FirstOrDefault(d => d.Status == DialogStatus.Talking && d.CallerID == opr.Username);
                        var clientDialog = dialogs.FirstOrDefault(d => d.Status == DialogStatus.Talking && d.CalleeID == opr.Username);
                        var status = account != null ? account.Status : SipAccountStatus.Offline;
                        string title = string.Format("Username: {0}\r\nFullname: {1}\r\nStatus: {2}", opr.Username, opr.Fullname, status);

                        if (clientDialog != null)
                            control.Icon = "TalkingClient";
                        else if (gatewayDialog != null)
                            control.Icon = "TalkingGateway";
                        else
                            control.Icon = "Idle";

                        switch (status)
                        {
                            case SipAccountStatus.Offline:
                                control.Icon = "Offline";
                                break;

                            case SipAccountStatus.Dialing:
                                control.Icon = "Ringing";
                                break;
                        }

                        if (clientDialog != null)
                        {
                            title += string.Format("\r\nCallerID: {0}", clientDialog.CallerID);
                            title += string.Format("\r\nCallTime: {0}", clientDialog.CallTime.TimeOfDay);
                        }

                        if (account != null && status != SipAccountStatus.Offline)
                            title += string.Format("\r\nEndPoint: {0}", account.SipEndPoint);

                        if (account != null && account.RegisterTime.HasValue)
                            title += string.Format("\r\nRegisterTime: {0}", account.RegisterTime.Value.TimeOfDay);

                        control.ToolTip = title;
                        control.TitleBlock.Content = opr.Fullname;
                    }
                }
            })));

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            VoipServiceClient.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
            Stop();
        }

        #endregion

        private static List<OperatorTeam> _Teams;
        public static List<OperatorTeam> Teams
        {
            get
            {
                if (_Teams == null)
                    ReloadTeams();
                return _Teams;
            }
            set
            {
                _Teams = value;
            }
        }

        public static void ReloadTeams()
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                Teams = new List<OperatorTeam>();
                foreach (var role in dc.Roles.Where(r => r.ParentID == Constants.TeamsRole))
                {
                    OperatorTeam operatorTeam = new OperatorTeam() {
                        TeamRole = role,
                    };

                    operatorTeam.Members = (from ur in dc.UserRoles
                                            where ur.RoleID == role.ID
                                            join u in dc.Users on ur.UserID equals u.ID
                                            select u).ToList();
                    Teams.Add(operatorTeam);
                }
            }
        }
    }
}
