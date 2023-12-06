using System.Windows.Controls;
using UMSV;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "Record", Icon = "images/record.png", Title = "ضبط پیام", ChildMode = ChildModes.SingleChild, GroupIndex = -1, Index = 1)]
    public partial class RecordNode : UserControl, IValidatable
    {
        public RecordNode()
        {
            InitializeComponent();
        }

        public ValidationResult Validate()
        {
            ValidationResult result;
            result = boxNoInputBox.Validate(MailboxType.Private);
         
            return result;
        }
        
    }
}
