using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pendar.Ums.Model;
namespace Pendar.Ums.CompositeNodes.UserControls
{

    public partial class CodeStatusSelector : TextBlock
    {
        public CodeStatusSelector()
        {
            InitializeComponent();
        }

        public UMSV.Schema.InvokeNode InvokeNode
        {
            get
            {
                return (UMSV.Schema.InvokeNode)GetValue(InvokeNodeProperty);
            }
            set
            {
                SetValue(InvokeNodeProperty, value);
            }
        }

        public static readonly DependencyProperty InvokeNodeProperty =
            DependencyProperty.Register("InvokeNode", typeof(UMSV.Schema.InvokeNode), typeof(CodeStatusSelector), new FrameworkPropertyMetadata()
            {
                BindsTwoWayByDefault = true
            });

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            CodeStatusDialog dlg = new CodeStatusDialog(InvokeNode.Clone())
            {
                //Owner = Window.GetWindow(this)
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            if (dlg.ShowDialog() == true)
            {
                dlg.InvokeNode.CopyTo(InvokeNode);
            }
        }

    }

}
