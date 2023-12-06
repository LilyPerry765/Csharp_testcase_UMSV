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

namespace UMSV.Forms
{
    /// <summary>
    /// Interaction logic for ConfigForm.xaml
    /// </summary>
    public partial class ConfigForm : UserControl, IFolderForm, IEditableForm
    {
        public ConfigForm()
        {
            InitializeComponent();
        }

        #region IEditableForm Members

        public bool Save()
        {
            if (!UMSV.Schema.Config.Save())
                return false;

            VoIPServiceClient_Plugin_UMSV.Default.ReloadUmsvConfig();
            return true;
        }

        #endregion

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                var graphs = dc.Graphs.ToList();
                Grid.ListDataSource.Add(UMSV.Schema.Config.DivertWaitGraphProperty, graphs);
                Grid.ListDataSource.Add(UMSV.Schema.Config.DefaultGraphProperty, graphs);
            }
            
            UMSV.Schema.Config.Load();
            Grid.SelectedObject = UMSV.Schema.Config.Default;
            dataGrid.ItemsSource = UMSV.Schema.Config.Default.Timeouts;
        }

        #endregion
    }
}
