using System.Windows.Controls;
using UMSV;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "JumpGraph", Icon = "images/service.png", GroupIndex = 3, Index = 1, Title = "انتقال به سرویس", ChildMode = ChildModes.Childless)]
    public partial class JumpGraphNode : UserControl
    {
        public JumpGraphNode()
        {
            InitializeComponent();
            LoadServices();
        }

        private void LoadServices()
        {
            UMSV.UmsDataContext db = new UMSV.UmsDataContext();
            servicesComboBox.ItemsSource = db.Graphs;
        }

       

    }
}
