using System.Windows.Controls;
using UMSV;
using UMSV.Schema;
using UMSV.ViewModels;
using System.Linq;
using System.Windows;

namespace Pendar.Ums.CompositeNodes
{

    [UMSV.CompositeNode(Tag = "Invoke", Icon = "images/invoke.png", Title = "عملیات داخلی", SubMenu = "عمليات پيشرفته", Index = 0, ChildMode = ChildModes.MultiChild)]
    public partial class InvokeNode : UserControl
    {
        private bool IsGettingDataContext;

        public InvokeNode()
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
                recordCheckBox.IsChecked = ViewModel.NodeData.RecordNodes.Any();
                IsGettingDataContext = false;
            }
        }

        #region Dynamic GetkeyNode Management

        private void keyCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsGettingDataContext)
            {
                recordCheckBox.IsChecked = false;
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

        #region Dynamic RecordNode Management

        private void recordCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!IsGettingDataContext)
            {
                keyCheckBox.IsChecked = false;
                AddRecordNode();
                recordStackmPanel.Children.Cast<FrameworkElement>().Last().BringIntoView();
            }
        }

        private void recordCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveRecordNode();
        }

        private void AddRecordNode()
        {
            ViewModel.NodeData.InvokeNodes[0].ID = string.Format("#{0}", ViewModel.GraphModel.GetNextID());
            ViewModel.NodeData.Items.Insert(0, new UMSV.Schema.RecordNode()
            {
                Timeout = 120,
                ClearDigits = true,
                CancelOnDisconnect = false,
                StopKey = "#",
                ID = ViewModel.NodeData.StartNode,
                TimeoutNode = ViewModel.NodeData.InvokeNodes[0].ID,
                TargetNode = ViewModel.NodeData.InvokeNodes[0].ID,
            });
            ViewModel.NodeData.LastNode = ViewModel.NodeData.InvokeNodes[0].ID;
            ViewModel.SendPropertyChanged("NodeData");
        }

        private void RemoveRecordNode()
        {
            ViewModel.NodeData.Items.Remove(ViewModel.NodeData.RecordNodes[0]);
            ViewModel.NodeData.LastNode = ViewModel.NodeData.InvokeNodes[0].ID = ViewModel.NodeData.StartNode;
            ViewModel.SendPropertyChanged("NodeData");
        }

        #endregion





    }
}
