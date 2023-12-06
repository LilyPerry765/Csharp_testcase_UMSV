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

namespace UMSV
{
    /// <summary>
    /// Interaction logic for BlackListForm.xaml
    /// </summary>
    public partial class BlackListForm : UserControl, IFolderForm, IEditableForm, IDataGridForm
    {
        public BlackListForm()
        {
            InitializeComponent();
        }

        UMSV.UmsDataContext dc = new UMSV.UmsDataContext();

        #region IFolderForm Members

        public void Initialize(FolderFormHelper helper)
        {
            dataGrid.ItemsSource = dc.SpecialPhones.Where(sp => sp.Type == (int)SpecialPhoneType.TemporaryBlackList || sp.Type == (int)SpecialPhoneType.BlackList);
            Users.ItemsSource = new FolderDataContext().Users;
        }

        #endregion

        #region IEditableForm Members

        public bool Save()
        {
            dc.SubmitChanges();
            return true;
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

        private void dataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            (e.NewItem as SpecialPhone).Type = (int)SpecialPhoneType.TemporaryBlackList;
            (e.NewItem as SpecialPhone).UserID = User.Current.ID;
            (e.NewItem as SpecialPhone).RegisterTime = new FolderDataContext().GetDate().Value;
        }
    }
}
