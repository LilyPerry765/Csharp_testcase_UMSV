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
using System.Net;
using Folder;
using UMSV.Schema;
using UMSV;
using System.Threading;
using System.IO;
using System.Xml.Linq;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Threading;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for SoftPhoneForm.xaml
    /// </summary>
    public partial class SoftPhoneForm : UserControl, IFolderForm, IDisposable
    {
        private SoftPhone SoftPhone = SoftPhone.CreateInstance();
        public string LastDialedNumber { get; set; }
        private List<UserRole> colleaguesOfCurrentUser = new List<UserRole>();
        public SoftPhoneForm()
        {
            InitializeComponent();
        }

        static SoftPhoneForm()
        {
            //The following GUID is VisibilityAccess identity of operators dashboard.
            if (Folder.User.IsInRole(Guid.Parse("FE8CDD32-915F-4A1A-8E8F-D24233272D0B")))
            {
                Folder.EMQ.ClientTransport.Default.Start();
                Folder.EMQ.ClientTransport.Default.ConnectToServer();
            }
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            Logger.WriteDebug("Initialize SoftPhoneForm ...");

            if (Folder.User.IsInRole(Guid.Parse("FE8CDD32-915F-4A1A-8E8F-D24233272D0B")))
            {
                VoipServiceClient.Default.OnResponseAccountsAndDialogs += new VoipServiceClient.ResponseAccountsAndDialogsHandler(Default_OnResponseAccountsAndDialogs);

                Start();
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

            SoftPhone.CallDisconnected += SoftPhone_CallDisconnected;
            SoftPhone.IncommingCall += SoftPhone_IncommingCall;
            SoftPhone.Registered += SoftPhone_Registered;
            SoftPhone.CallRejected += SoftPhone_CallRejected;
            SoftPhone.CallAnswered += SoftPhone_CallAnswered;
            SoftPhone.UnRegistered += SoftPhone_UnRegistered;
            SoftPhone.TransferFailed += SoftPhone_TransferFailed;
            DndButton.IsChecked = SoftPhone.DoNotDisturb;

            helper.Closing += helper_Closing;
        }

        void helper_Refresh(object sender, RefreshEventArgs e)
        {
            RequestDashboardInfoFromServer(null);
        }

        void Default_OnResponseAccountsAndDialogs(DateTime serverTime, List<SipAccount> accounts, List<SipDialog> dialogs)
        {
            Dispatcher.Invoke(((Action)(() =>
            {
                var targetTeams = Teams.Where(t => Folder.User.Current.AllRoles.Where(r => r.ParentID == Guid.Parse("3CED18BF-D71A-4A1E-BD8D-503ACC339BF9")).Select(s => s.ID).Contains(t.TeamRole.ID));

                Logger.WriteImportant("teams: {0}, target: {1}", Teams.Count, targetTeams.Count());

                int queue = 0;
                int calls = 0;
                int dnd = 0;
                int online = 0;
                foreach (OperatorTeam team in targetTeams)
                {
                    /////////////////////////////////////////////////
                    //QueuePanel.Children.Clear();
                    //var ondivertSortDialogs = from dialog in dialogs
                    //                          orderby dialog.QueueEnterTime
                    //                          select dialog;

                    //foreach (var dialog in ondivertSortDialogs.Where(d =>
                    //    !string.IsNullOrEmpty(d.DivertTargetTeam) &&
                    //    Guid.Parse(d.DivertTargetTeam) == team.TeamRole.ID &&
                    //    d.Status == DialogStatus.WaitForDiverting))
                    //{
                    //    QueuePanel.Children.Add(new TextBox()
                    //    {
                    //        Background = Brushes.Yellow,
                    //        Foreground = Brushes.Black,
                    //        BorderBrush = Brushes.Gray,
                    //        Text = dialog.CallerID,
                    //        ToolTip = string.Format("زمان تماس: {0}\r\n زمان ورود به صف: {1}", dialog.CallTime.TimeOfDay, dialog.QueueEnterTime),
                    //        Margin = new Thickness(3),
                    //    });
                    //}
                    ////////////////////////////////////////////////

                    queue += dialogs.Count(d => !string.IsNullOrEmpty(d.DivertTargetTeam) && Guid.Parse(d.DivertTargetTeam) == team.TeamRole.ID && d.Status == DialogStatus.WaitForDiverting);

                    var teamAccounts = accounts.Where(a => team.Members.Select(u => u.Username).Contains(a.UserID));

                    calls += teamAccounts.Count(d => d.Status == SipAccountStatus.Talking || d.Status == SipAccountStatus.Hold);
                    dnd += teamAccounts.Count(d => d.Status == SipAccountStatus.DND);
                    online += teamAccounts.Count(d => d.Status != SipAccountStatus.Offline);
                }

                SevenSegmentQueue.Value = queue;
                SevenSegmentCalls.Value = calls;
                SevenSegmentDNDUsers.Value = dnd;
                SevenSegmentOnlineUsers.Value = online;
            })));

            if (requestInfoFromServerTimer != null)
                requestInfoFromServerTimer.Change(UMSV.Schema.Config.Default.OperatorsMonitoringRefreshInterval, -1);
        }

        void SoftPhone_TransferFailed(object sender, EventArgs e)
        {
            ActionText = "Talking ...";
            SoftPhone.UnHold();
        }

        void SoftPhone_UnRegistered(object sender, UnRegisteredEventArgs e)
        {
            KeysText = string.Empty;
            ActionText = string.Empty;
            DisplayPanel.Background = Brushes.Red;

            Dispatcher.Invoke((Action)(() =>
            {
                QueuePanel.Visibility = System.Windows.Visibility.Collapsed;
                PhonePanel.Visibility = System.Windows.Visibility.Collapsed;
                UserLoginButton.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        void SoftPhone_CallAnswered(object sender, EventArgs e)
        {
            ActionText = "Talking ...";
        }

        void SoftPhone_CallRejected(object sender, EventArgs e)
        {
            ActionText = string.Empty;
            KeysText = string.Empty;
            CallAlertWindow.HideCallAlert();
        }

        void SoftPhone_Registered(object sender, EventArgs e)
        {
            DisplayPanel.Background = Brushes.Black;

            Dispatcher.Invoke((Action)(() =>
            {
                UserLoginButton.IsEnabled = true;
                UserLoginButton.Visibility = System.Windows.Visibility.Collapsed;
                PhonePanel.Visibility = System.Windows.Visibility.Visible;
                //QueuePanel.Visibility = System.Windows.Visibility.Visible;
            }));
        }

        void SoftPhone_CallConnected(object sender, EventArgs e)
        {
            ActionText = "Connected";
        }

        void SoftPhone_IncommingCall(object sender, IncommingCallEventArgs e)
        {
            ActionText = "Ringing ...";
            if (Config.Default.SoftPhoneShowIncommingCallCallerID)
                KeysText = e.Dialog.CallerID;
        }

        private string ActionText
        {
            get
            {
                return (string)ActionLabel.Content;
            }
            set
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    ActionLabel.Content = value;
                }));
            }
        }

        private string KeysText
        {
            get
            {
                return KeysTextbox.Text;
            }
            set
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    KeysTextbox.Text = value;
                }));
            }
        }

        void SoftPhone_CallDisconnected(object sender, EventArgs e)
        {
            ActionText = string.Empty;
            KeysText = string.Empty;
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            if (Folder.User.IsInRole(Guid.Parse("FE8CDD32-915F-4A1A-8E8F-D24233272D0B")))
            {
                VoipServiceClient.Default.OnResponseAccountsAndDialogs -= Default_OnResponseAccountsAndDialogs;
                Stop();
            }
            SoftPhone.Stop();
        }

        #endregion

        private void RequestDashboardInfoFromServer(object state)
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
            requestInfoFromServerTimer = new Timer(RequestDashboardInfoFromServer, null, 0, -1);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (HoldButton.IsChecked)
                HoldButton.IsChecked = false;

            switch (SoftPhone.AccountStatus)
            {
                case SipAccountStatus.Dialing:
                    SoftPhone.DisconnectCall();
                    SoftPhone.CallAlertWindow_Rejected(sender, e);
                    break;

                case SipAccountStatus.Talking:
                case SipAccountStatus.Hold:
                    ActionText = string.Empty;
                    KeysText = string.Empty;
                    SoftPhone.DisconnectCall();
                    break;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            UserLoginButton.IsEnabled = false;
            Logger.WriteView("LoginButton_Click");
            DndButton.IsChecked = false;

            KeysText = string.Empty;
            if (Config.Default.AssumeSessionUserAsSoftPhoneUser)
            {
                if (SoftPhone.Start(User.Current.Username, User.Current.Password))
                    return;
            }
            else
            #region Ask username and Password
            {
                UserLoginWindow window = new UserLoginWindow();
                window.UsernameTextbox.Text = User.Current[Constants.UserProfileKey_VoipUsername] ?? string.Empty;

                if (window.ShowDialog() == true)
                {
                    if (User.Current[Constants.UserProfileKey_VoipUsername] == window.UsernameTextbox.Text && User.Current[Constants.UserProfileKey_VoipPassword] == window.PasswordTextbox.Password)
                    {
                        if (SoftPhone.Start(window.UsernameTextbox.Text, window.PasswordTextbox.Password))
                            return;
                    }
                    else
                    {
                        Folder.MessageBox.ShowError("نام کاربری یا کلمه عبور نا معتبر است");
                        return;
                    }
                }
            }
            #endregion

            UserLoginButton.IsEnabled = true;
        }

        private void StartDial()
        {
            LastDialedNumber = KeysText;
            if (SoftPhone.Call(KeysText))
                ActionText = "Dialing ...";
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            StartDial();
        }

        private void KeysTextbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                StartDial();
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            char key = (sender as JellyButton).Text.First();
            KeysText += key;

            SoftPhone.SendDtmf(key);
        }

        private void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            TransferForm form = new TransferForm();
            if (form.ShowDialog() == true)
            {
                SoftPhone.Transfer(form.PhoneNumberTextbox.Text);
                SoftPhone.Hold();
                ActionText = string.Format("Transfering to '{0}' ...", form.PhoneNumberTextbox.Text);
            }
        }

        private void LogoffButton_Click(object sender, RoutedEventArgs e)
        {
            SoftPhone.Stop();
        }

        private void DndButton_Changed(object sender, RoutedEventArgs e)
        {
            SoftPhone.DoNotDisturb = DndButton.IsChecked;
            DisplayPanel.Background = DndButton.IsChecked ? Brushes.Red : Brushes.Black;

            if (DndButton.IsChecked)
            {
                SoftPhone.DndStart();
            }
            else
            {
                SoftPhone.DndEnd();
            }
            KeysTextbox.Focus();
        }

        private void HoldButton_Changed(object sender, RoutedEventArgs e)
        {
            if (HoldButton.IsChecked)
                SoftPhone.Hold();
            else
                SoftPhone.UnHold();
        }

        private void AddToBlackListButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(KeysText))
                return;

            AddToBlackListWindow form = new AddToBlackListWindow();
            form.NumberTextbox.Text = KeysText;
            if (form.ShowDialog().Value)
                Folder.MessageBox.ShowInfo("شماره تلفن '{0}' در لیست سیاه قرار گرفت.", KeysText);
        }

        public void Dispose()
        {
            SoftPhone.Stop();
        }

        private void RedialButton_Click(object sender, RoutedEventArgs e)
        {
            KeysText = LastDialedNumber;
        }

        #region Team
        private static List<OperatorTeam> _Teams;
        private static List<OperatorTeam> Teams
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

        private static void ReloadTeams()
        {
            using (FolderDataContext dc = new FolderDataContext())
            {
                Teams = new List<OperatorTeam>();
                foreach (var role in dc.Roles.Where(r => r.ParentID == Guid.Parse("3CED18BF-D71A-4A1E-BD8D-503ACC339BF9")))
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
        #endregion
    }

    class OperatorTeam
    {
        public Role TeamRole;
        public List<User> Members = new List<User>();
    }
}
