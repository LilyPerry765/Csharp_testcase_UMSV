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
using UMSV;
using UMSV.Schema;

namespace UMSV
{
    /// <summary>
    /// Interaction logic for SipAccountsForm.xaml
    /// </summary>
    public partial class SipAccountsForm : UserControl, IEditableForm, IDataGridForm, IFolderForm
    {
        public SipAccountsForm()
        {
            InitializeComponent();
        }

        #region IEditableForm Members

        public bool Save()
        {
            return Config.Save();
        }

        #endregion

        #region IDataGridForm Members

        public DataGrid DataGrid
        {
            get
            {
                return this.dataGrid;
            }
        }

        #endregion

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            UMSV.Schema.Config.Load();
            dataGrid.ItemsSource = null;
        }

        #endregion
    }
}
