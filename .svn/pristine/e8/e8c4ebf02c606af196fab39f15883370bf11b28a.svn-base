using System.Windows.Controls;
using UMSV;
using System.Text.RegularExpressions;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "RecordToMailbox", Icon = "images/mailBoxRecord.png", Title = "ضبط در صندوق عمومی", ChildMode = ChildModes.SingleChild, GroupIndex = 2, Index = 2)]
    public partial class RecordToMailBoxNode : UserControl, IValidatable
    {
        public RecordToMailBoxNode()
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
