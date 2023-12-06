using System.Windows.Controls;
using UMSV;
using UMSV.ViewModels;
using System.Linq;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "InfoTable", Icon = "images/infoTable.png", Title = "جدول اطلاعات گویا", SubMenu = "عمليات پيشرفته", Index = 1)]
    public partial class InfoTableNode : UserControl
    {
        public InfoTableNode()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is CompositeNodeViewModel)
                (DataContext as CompositeNodeViewModel).Removing += new System.EventHandler(InfoTableNode_Removing);
        }

        void InfoTableNode_Removing(object sender, System.EventArgs e)
        {
            DeleteInfoTableFromDB();
        }

        private void DeleteInfoTableFromDB()
        {
            UMSV.UmsDataContext db = new UMSV.UmsDataContext();
            var infoTable = db.InfoTables.FirstOrDefault(it => it.ID.ToString() == infoTableSelector.SelectedInfoTableID);
            if (infoTable != null)
            {
                db.InfoTables.DeleteOnSubmit(infoTable);
                db.SubmitChanges();
            }
        }


    }
}
