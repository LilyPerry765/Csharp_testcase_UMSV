using System.Windows.Controls;
using UMSV;
using System.Text.RegularExpressions;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "FollowUp", Icon = "images/follow.png", Title = "پیگیری", ChildMode = ChildModes.SingleChild, GroupIndex = 1, Index = 2)]
    public partial class FollowUpNode : UserControl, IValidatable
    {
        public FollowUpNode()
        {
            InitializeComponent();
        }

        public ValidationResult Validate()
        {
            ValidationResult result;
            result = boxNoInputBox.Validate();
            return result;
        }

    }
}
