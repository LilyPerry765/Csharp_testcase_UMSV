using System.Windows.Controls;
using UMSV;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "GetKey", Icon = "images/key.png", Title = "انتخاب مبتنی بر کليد ورودی", GroupIndex =0, Index = 0, ChildMode = ChildModes.MultiChild)]
    public partial class GetKeyNode : UserControl
    {
        public GetKeyNode()
        {
            InitializeComponent();
        }

    }
}
