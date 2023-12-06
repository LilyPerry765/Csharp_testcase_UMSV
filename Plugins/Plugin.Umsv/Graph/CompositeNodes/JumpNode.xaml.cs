using System.Windows.Controls;
using UMSV;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "JumpNode", Icon = "images/goto.png", GroupIndex = 3, Index = 0, Title = "انتقال به گره", ChildMode = ChildModes.Childless)]
    public partial class JumpNode : UserControl
    {
        public JumpNode()
        {
            InitializeComponent();
        }
    }
}
