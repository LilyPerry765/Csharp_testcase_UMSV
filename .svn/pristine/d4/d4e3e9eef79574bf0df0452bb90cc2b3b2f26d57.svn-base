using System.Windows.Controls;
using UMSV;
using System.Text.RegularExpressions;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "PlayFromMailbox", Icon = "images/mailBoxPlay.png", Title = "پخش از صندوق عمومی", ChildMode = ChildModes.SingleChild, GroupIndex = 2, Index = 1)]
    public partial class PlayFromMailboxNode : UserControl, IValidatable
    {
        public PlayFromMailboxNode()
        {
            InitializeComponent();
        }

        public ValidationResult Validate()
        {
            ValidationResult result;
            result = boxNoInputBox.Validate(MailboxType.Public);
            return result;
        }
    }
}
