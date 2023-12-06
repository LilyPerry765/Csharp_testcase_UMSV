using System.Windows.Controls;
using UMSV;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "CallerIDMatch", Icon = "images/call.png", GroupIndex = 0, Index = 2, Title = "انتخاب مبتنی بر مبدا تماس", ChildMode = ChildModes.MultiChild)]
    public partial class MatchCallerID : UserControl
    {
        public MatchCallerID()
        {
            InitializeComponent();
        }
    }
}
