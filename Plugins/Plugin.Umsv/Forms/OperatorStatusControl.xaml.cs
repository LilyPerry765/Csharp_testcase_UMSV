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
using Enterprise;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for AccountUserControl.xaml
    /// </summary>
    public partial class OperatorStatusControl : UserControl
    {
        public OperatorStatusControl()
        {
            softPhone.Registered += softPhone_Registered;
            InitializeComponent();
        }

        private SoftPhone _softPhone;
        public SoftPhone softPhone
        {
            get
            {
                if (_softPhone == null)
                    _softPhone = SoftPhone.CreateInstance();

                return _softPhone;
            }
        }

        private UMSV.Schema.SipAccount account;
        public UMSV.Schema.SipAccount Account
        {
            get
            {
                return account;
            }

            set
            {
                account = value;

                if (!string.IsNullOrEmpty(value.EavesdropperUserId))
                {
                    if (EavesdroppingYellowImage.Visibility == Visibility.Visible)
                    {
                        EavesdroppingYellowImage.Visibility = Visibility.Collapsed;
                        EavesdroppingRedImage.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        EavesdroppingYellowImage.Visibility = Visibility.Visible;
                        EavesdroppingRedImage.Visibility = Visibility.Collapsed;
                    }

                    if (account.Status == SipAccountStatus.Offline)
                        VoIPServiceClient_Plugin_UMSV.Default.ReleaseEavesdropper(account.UserID);
                }
                else
                {
                    EavesdroppingYellowImage.Visibility = Visibility.Collapsed;
                    EavesdroppingRedImage.Visibility = Visibility.Collapsed;
                }
            }
        }

        public SipAccountStatus AccountStatus
        {
            set
            {
                OperatorContrlContextMenu.Visibility = Visibility.Collapsed;


                TalkingImage.Visibility = Visibility.Collapsed;
                TalkingExternalImage.Visibility = Visibility.Collapsed;
                TalkingRingingImage.Visibility = Visibility.Collapsed;
                HoldImage.Visibility = Visibility.Collapsed;
                DndImage.Visibility = Visibility.Collapsed;
                Opacity = 1;

                switch (value)
                {
                    case SipAccountStatus.Offline:
                        Opacity = .35;
                        break;

                    case SipAccountStatus.DND:
                        DndImage.Visibility = Visibility.Visible;
                        break;

                    case SipAccountStatus.Dialing:
                        TalkingRingingImage.Visibility = Visibility.Visible;
                        break;

                    case SipAccountStatus.Hold:
                        HoldImage.Visibility = Visibility.Visible;
                        break;

                    case SipAccountStatus.Idle:
                        OperatorContrlContextMenu.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        public DialogType DialogType
        {
            set
            {
                switch (value)
                {
                    case DialogType.GatewayIncomming:
                    case DialogType.ClientIncomming:
                        TalkingImage.Visibility = Visibility.Visible;
                        OperatorContrlContextMenu.Visibility = Visibility.Visible;
                        break;

                    case DialogType.ClientOutgoing:
                    case DialogType.GatewayOutgoing:
                        TalkingExternalImage.Visibility = Visibility.Visible;
                        OperatorContrlContextMenu.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void EavesdroppingItem_Click(object sender, RoutedEventArgs e)
        {
            if (Account != null)
            {
                if (!string.IsNullOrEmpty(Account.EavesdropperUserId) && Account.EavesdropperUserId != Folder.User.Current.Username)
                {
                    Folder.MessageBox.ShowWarning(string.Format("کاربر '{0}' در حال شنود مکالمه های این اپراتور می باشد و امکان شنود برای شما وجود ندارد.", Account.EavesdropperUserId));
                    return;
                }

                if (!OperatorTeamsMonitoringForm.ControlSoftphoneRegistered)
                {
                    softPhone.Start(Folder.User.Current.Username, Folder.User.Current.Password);
                }
                else
                {
                    VoIPServiceClient_Plugin_UMSV.Default.DisconnectEavesdroppingCall(Folder.User.Current.Username);

                    VoIPServiceClient_Plugin_UMSV.Default.FlushEavesdropper(Folder.User.Current.Username);

                    VoIPServiceClient_Plugin_UMSV.Default.SetAccountEavesdropper(Folder.User.Current.Username, Account.UserID);

                    Logger.WriteImportant("Eavesdropper already registered. Switched eavesdropping: {0} => ", Folder.User.Current.Username, Account.UserID);
                }
            }
        }

        void softPhone_Registered(object sender, EventArgs e)
        {
            OperatorTeamsMonitoringForm.ControlSoftphoneRegistered = true;
            VoIPServiceClient_Plugin_UMSV.Default.SetAccountEavesdropper(Folder.User.Current.Username, Account.UserID);
            Logger.WriteImportant("Eavesdropper registered: {0} => {1}", Folder.User.Current.Username, Account.UserID);
        }

    }
}

