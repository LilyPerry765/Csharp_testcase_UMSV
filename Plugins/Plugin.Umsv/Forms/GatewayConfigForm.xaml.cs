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
using Enterprise;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for MediaGatewayConfigForm.xaml
    /// </summary>
    public partial class GatewayConfigForm : UserControl, IFolderForm, IEditableForm, IDataGridForm
    {
        public GatewayConfigForm()
        {
            InitializeComponent();
        }

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            MediaGatewayConfig.Load();
            DataGrid.ItemsSource = MediaGatewayConfig.Default.Ciscos;
            DevicesColumn.ItemsSource = MediaGatewayConfig.Default.Ciscos;
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseInquiryCiscoLinkState += new VoIPServiceClient_Plugin_UMSV.ResponseInquiryCiscoLinkStateHandler(Default_OnResponseInquiryCiscoLinkState);

            LinksGrid.ItemsSource = MediaGatewayConfig.Default.Links;
            helper.Closing += new EventHandler<FormClosingEventArgs>(helper_Closing);
        }

        void helper_Closing(object sender, FormClosingEventArgs e)
        {
            MediaGatewayConfig.Load();
            VoIPServiceClient_Plugin_UMSV.Default.OnResponseInquiryCiscoLinkState -= new VoIPServiceClient_Plugin_UMSV.ResponseInquiryCiscoLinkStateHandler(Default_OnResponseInquiryCiscoLinkState);
        }

        void Default_OnResponseInquiryCiscoLinkState(Guid deviceID, int slot, int port, string result)
        {
            var link = MediaGatewayConfig.Default.Links.FirstOrDefault(l => l.DeviceID == deviceID && l.Slot == slot &&
                           l.Port == port);
            link.CurrentState = result;
            link.IsEnabled = !link.CurrentState.ToLower().Contains("down");
        }

        #endregion

        #region IEditableForm Members

        public bool Save()
        {
            if (!MediaGatewayConfig.Save())
                return false;

            VoIPServiceClient_Plugin_UMSV.Default.ReloadGatewayConfig();
            return true;
        }

        #endregion

        private void NoShutdown_Click(object sender, RoutedEventArgs e)
        {
            var link = LinksGrid.CurrentCellItem<MediaGatewayConfigLink>();
            if (link == null)
                return;

            var cisco = MediaGatewayConfig.Default.Ciscos.First(d => d.DeviceID == link.DeviceID);
            if (MessageBoxResult.Yes == Folder.MessageBox.ShowQuestion("آیا نسبت به فعال کردن لینک '{0}' مطمئن هستید؟", link.Title))
            {
                link.CurrentState = "در حال بروزآوری ...";
                VoIPServiceClient_Plugin_UMSV.Default.Call("ShutdownCiscoLink",
                    cisco.DeviceID, cisco.Address, cisco.Password, cisco.EnablePassword, link.Slot, link.Port, false);
            }
        }

        private void Shutdown_Click(object sender, RoutedEventArgs e)
        {
            var link = LinksGrid.CurrentCellItem<MediaGatewayConfigLink>();
            if (link == null)
                return;

            var cisco = MediaGatewayConfig.Default.Ciscos.First(d => d.DeviceID == link.DeviceID);
            if (MessageBoxResult.Yes == Folder.MessageBox.ShowQuestion("آیا نسبت به غیر فعال کردن لینک '{0}' مطمئن هستید؟", link.Title))
            {
                link.IsEnabled = false;
                link.CurrentState = "در حال بروزآوری ...";
                VoIPServiceClient_Plugin_UMSV.Default.Call("ShutdownCiscoLink",
                    cisco.DeviceID, cisco.Address, cisco.Password, cisco.EnablePassword, link.Slot, link.Port, true);
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            MediaGatewayConfig.Default.Links.ForEach(link => RefreshLinkState(link));
        }

        private void RefreshLink_Click(object sender, RoutedEventArgs e)
        {
            var link = LinksGrid.CurrentCellItem<MediaGatewayConfigLink>();
            if (link == null)
                return;

            RefreshLinkState(link);
        }

        private void RefreshLinkState(MediaGatewayConfigLink link)
        {
            link.CurrentState += " (در حال بروزآوری ...)";
            var device = MediaGatewayConfig.Default.Ciscos.FirstOrDefault(d => d.DeviceID == link.DeviceID);
            VoIPServiceClient_Plugin_UMSV.Default.Call("InquiryCiscoLinkState", link.DeviceID, device.Address, device.Password, device.EnablePassword, link.Slot, link.Port);
        }

        #region IDataGridForm Members

        public DataGrid DataGrid
        {
            get
            {
                return this.dataGrid;
            }
        }

        #endregion
    }
}
