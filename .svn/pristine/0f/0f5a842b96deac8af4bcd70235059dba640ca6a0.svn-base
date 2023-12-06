using System.Windows.Controls;
using UMSV;
using UMSV.Schema;
using UMSV.ViewModels;
using System.Linq;
using System.Windows;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "FaxReceive", Icon = "images/fax1.png", Title = "ارسال فکس", GroupIndex = 3, Index = 3, ChildMode = ChildModes.Childless)]
    public partial class FaxReceiveNode : UserControl
    {
        private bool IsGettingDataContext;

        public FaxReceiveNode()
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
            ViewModel.NodeData.FaxNodes[0].ID = string.Format("#{0}", ViewModel.GraphModel.GetNextID());
            ViewModel.NodeData.Items.Insert(0, new UMSV.Schema.GetKeyNode()
            {
                ClearDigits = false,
                Timeout = 5,
                MinDigits = 0,
                MaxDigits = 1,
                EndKey = "#",
                ID = ViewModel.NodeData.StartNode,
                TimeoutNode = ViewModel.NodeData.FaxNodes[0].ID,
                MaxDigitsNode = ViewModel.NodeData.FaxNodes[0].ID
            });
            ViewModel.NodeData.LastNode = ViewModel.NodeData.FaxNodes[0].ID;
            ViewModel.SendPropertyChanged("NodeData");
        }

        private void RemoveGetKeyNode()
        {
            ViewModel.NodeData.Items.Remove(ViewModel.NodeData.GetKeyNodes[0]);
            ViewModel.NodeData.LastNode = ViewModel.NodeData.FaxNodes[0].ID = ViewModel.NodeData.StartNode;
            ViewModel.SendPropertyChanged("NodeData");
        }
    }
}
