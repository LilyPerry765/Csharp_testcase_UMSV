using System.Windows.Controls;
using UMSV;
using UMSV.Schema;
using UMSV.ViewModels;
using System.Linq;
using System.Windows;

namespace Pendar.Ums.CompositeNodes
{
    [UMSV.CompositeNode(Tag = "WebMethod", Icon = "images/WebMethod.png", Title = "فراخوانی Web Service", SubMenu = "عمليات پيشرفته", Index = 0, ChildMode = ChildModes.MultiChild, ViewRole = "F5B41D1B-5800-4196-8FCE-D7DF652A5D78")]
    public partial class WebMethodNode : UserControl
    {
        private bool IsGettingDataContext;

        public WebMethodNode()
        {
            InitializeComponent();
        }

        private CompositeNodeViewModel ViewModel
        {
            get
            {
                return this.DataContext as CompositeNodeViewModel;
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ViewModel != null)
            {
                IsGettingDataContext = true;
                keyCheckBox.IsChecked = ViewModel.NodeData.GetKeyNodes.Any();
                IsGettingDataContext = false;
            }
        }

        #region Dynamic GetkeyNode Management

        private void keyCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsGettingDataContext)
            {
                AddGetKeyNode();
                keyStackPanel.Children.Cast<FrameworkElement>().Last().BringIntoView();
            }
        }

        private void keyCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveGetKeyNode();
        }

        private void AddGetKeyNode()
        {
            ViewModel.NodeData.InvokeNodes[0].ID = string.Format("#{0}", ViewModel.GraphModel.GetNextID());
            ViewModel.NodeData.Items.Insert(0, new UMSV.Schema.GetKeyNode()
            {
                ClearDigits = false,
                Timeout = 5,
                MinDigits = 0,
                MaxDigits = 1,
                EndKey = "#",
                ID = ViewModel.NodeData.StartNode,
                TimeoutNode = ViewModel.NodeData.InvokeNodes[0].ID,
                MaxDigitsNode = ViewModel.NodeData.InvokeNodes[0].ID
            });
            ViewModel.NodeData.LastNode = ViewModel.NodeData.InvokeNodes[0].ID;
            ViewModel.SendPropertyChanged("NodeData");
        }

        private void RemoveGetKeyNode()
        {
            ViewModel.NodeData.Items.Remove(ViewModel.NodeData.GetKeyNodes[0]);
            ViewModel.NodeData.LastNode = ViewModel.NodeData.InvokeNodes[0].ID = ViewModel.NodeData.StartNode;
            ViewModel.SendPropertyChanged("NodeData");
        }

        #endregion
    }
}
