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
    public partial class OperatorTeamsMonitoringForm : UserControl, IFolderForm
    {
        public OperatorTeamsMonitoringForm()
        {
            InitializeComponent();
        }

        static OperatorTeamsMonitoringForm()
        {
            OperatorTeamsMonitoringForm.ReloadTeams();
            Folder.EMQ.ClientTransport.Default.Start();
            Folder.EMQ.ClientTransport.Default.ConnectToServer();
        }

        public static bool ControlSoftphoneRegistered = false;

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
            foreach (var team in Teams.OrderBy(t => t.TeamRole.Name))
            {
                var groupBox = new Expander()
                {
                    Header = team.TeamRole.Name,
                    FontWeight = FontWeights.Bold,
                    Tag = team.TeamRole,
                };

                Panel.Children.Add(groupBox);

                StackPanel panel = new StackPanel();
                groupBox.Content = panel;

                panel.Children.Add(new WrapPanel());
                panel.Children.Add(new Border()
                {
                    Margin = new Thickness(3),
                    CornerRadius = new CornerRadius(3),
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(1),
                    Child = new WrapPanel()
                    {
                        Orientation = Orientation.Horizontal,
                    }
                });

                foreach (var opr in team.Members)
                {
                    OperatorStatusControl control = new OperatorStatusControl();
                    control.Tag = opr;
                    string title = string.Format("Username: {0}\r\nFullname: {1}", opr.Username, opr.Fullname);
                    control.Effect = new GrayscaleEffect();
                    control.ToolTip = title;
                    control.TitleBlock.Text = opr.Fullname;
                    (panel.Children[0] as WrapPanel).Children.Add(control);
                }
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
                foreach (Expander groupBox in Panel.Children)
                {
                    var group = groupBox.Content as StackPanel;

                    var queuePanel = (group.Children[1] as Border).Child as WrapPanel;
                    queuePanel.Children.Clear();
                    var ondivertSortDialogs = from dialog in dialogs
                                              orderby dialog.QueueEnterTime
                                              select dialog;

                    foreach (var dialog in ondivertSortDialogs.Where(d =>
                        !string.IsNullOrEmpty(d.DivertTargetTeam) &&
                        Guid.Parse(d.DivertTargetTeam) == (groupBox.Tag as Role).ID &&
                        d.Status == DialogStatus.WaitForDiverting))
                    {
                        queuePanel.Children.Add(new TextBox()
                        {
                            Background = Brushes.Yellow,
                            Foreground = Brushes.Black,
                            BorderBrush = Brushes.Gray,
                            Text = dialog.CallerID,
                            ToolTip = string.Format("زمان تماس: {0}\r\n زمان ورود به صف: {1}", dialog.CallTime.TimeOfDay, dialog.QueueEnterTime),
                            Margin = new Thickness(3),
                        });
                    }

                    foreach (OperatorStatusControl control in (group.Children[0] as WrapPanel).Children)
                    {
                        var opr = control.Tag as User;
                        var account = accounts.FirstOrDefault(a => a.UserID == opr.Username);
                        var dialog = dialogs.FirstOrDefault(d => d.Status == DialogStatus.Talking &&
                            (d.CalleeID == opr.Username || d.CallerID == opr.Username));
                        var status = account != null ? account.Status : SipAccountStatus.Offline;

                        if (status == SipAccountStatus.Offline && dialog != null)
                            status = SipAccountStatus.Talking;
                        control.AccountStatus = status;
                        string title;
                        control.Effect = null;

                        if (account != null)
                        {
                            title = string.Format("{1}\r\nUsername: {0}\r\nStatus: {2}\r\nEndPoint: {3}", opr.Username, opr.Fullname, account.Status, account.SipEndPoint);
                            control.Account = account;

                            if (!string.IsNullOrEmpty(account.EavesdropperUserId))
                                title += string.Format("\r\nEavesdropper: {0}", account.EavesdropperUserId);

                            //if (!string.IsNullOrEmpty(account.EavesdropperUserId) && (account.Status == SipAccountStatus.Offline || account.Status == SipAccountStatus.Idle))
                            //{
                            //    VoIPServiceClient_Plugin_UMSV.Default.DisconnectEavesdropperDialog(Folder.User.Current.Username);
                            //}
                        }
                        else
                            title = string.Format("{1}\r\nUsername: {0}", opr.Username, opr.Fullname);


                        if (dialog != null)
                        {
                            title += string.Format("\r\nCallerID: {0}\r\nCalleeID: {2}\r\nCallTime: {1}", dialog.CallerID, dialog.CallTime.TimeOfDay, dialog.CalleeID);
                            control.DialogType = dialog.DialogType;
                        }


                        if (account != null && account.RegisterTime.HasValue)
                            title += string.Format("\r\nRegisterTime: {0}", account.RegisterTime.Value.TimeOfDay);

                        control.ToolTip = title;
                        control.TitleBlock.Text = opr.Fullname;
                    }
                }
            })));

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            FlushEavesdroppers();
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
            Stop();
        }

        #endregion

        private void FlushEavesdroppers()
        {
            Dispatcher.Invoke(((Action)(() =>
            {
                foreach (Expander groupBox in Panel.Children)
                {
                    var group = groupBox.Content as StackPanel;

                    foreach (OperatorStatusControl control in (group.Children[0] as WrapPanel).Children)
                    {
                        if (control.Account != null && control.Account.EavesdropperUserId == Folder.User.Current.Username)
                        {
                            VoIPServiceClient_Plugin_UMSV.Default.DisconnectEavesdroppingCall(Folder.User.Current.Username);
                            VoIPServiceClient_Plugin_UMSV.Default.FlushEavesdropper(control.Account.EavesdropperUserId);
                            ControlSoftphoneRegistered = false;
                            control.softPhone.AccountStatus = SipAccountStatus.Idle;
                            control.softPhone.UnRegister();
                            break;
                        }
                        else if (ControlSoftphoneRegistered)
                        {
                            ControlSoftphoneRegistered = false;
                            control.softPhone.AccountStatus = SipAccountStatus.Idle;
                            control.softPhone.UnRegister();
                        }
                    }
                }
            })));
        }

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
                    OperatorTeam operatorTeam = new OperatorTeam()
                    {
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

    public class OperatorTeam
    {
        public Role TeamRole;
        public List<User> Members = new List<User>();
    }
}
