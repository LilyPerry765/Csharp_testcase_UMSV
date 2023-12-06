using System.Windows.Controls;
using UMSV;
using System.Text.RegularExpressions;
using Pendar.Ums.Model;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "Ask", Icon = "images/question.png", Title = "پرسش", ChildMode = ChildModes.SingleChild, GroupIndex = 1, Index = 1)]
    public partial class AskNode : UserControl, IValidatable
    {
        public AskNode()
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
