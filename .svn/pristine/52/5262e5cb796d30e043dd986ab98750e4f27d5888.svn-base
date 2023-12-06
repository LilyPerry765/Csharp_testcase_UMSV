using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using UMSV.ViewModels;
using System.Text.RegularExpressions;
using System;
using System.Windows.Data;

namespace Pendar.Ums.CompositeNodes.UserControls
{
    /// <summary>
    /// Interaction logic for TargetSelector.xaml
    /// </summary>
    public partial class TargetSelector : UserControl
    {
        private static List<TargetSelector> instances = new List<TargetSelector>();

        public TargetSelector()
        {
            InitializeComponent();
            instances.Add(this);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is CompositeNodeViewModel)
            {
                CompositeNode.GraphModel.NodePicked += new Action<UMSV.TreeNode>(TargetSelector_NodePicked);
                CompositeNode.GraphModel.NodeRemoving += new Action<UMSV.TreeNode>(GraphModel_NodeRemoving);
                CompositeNode.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(CompositeNode_PropertyChanged);
            }
        }

        void CompositeNode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsTargetLocked":
                    IsEnabled = IsAlwaysEnabled || !CompositeNode.IsTargetLocked;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// When the TargetSelector is not used for NextNode, it will never be disabled when a sub-node is added as the next node.
        /// Setting this property marks this TargetSelector as such nodes, so that it is not disabled when CompositeNode.IsTargetLocked is true.
        /// This property is declared because It was difficult to determine whether this target selector is bound to the LastNode or another Node.
        /// </summary>
        public bool IsAlwaysEnabled
        {
            get;
            set;
        }

        void GraphModel_NodeRemoving(UMSV.TreeNode removingNode)
        {
            foreach (var item in instances)
            {
                removingNode.CascadeAction(n =>
                {
                    if (item.SelectedTarget == n.GraphNode.StartNode)
                        item.SelectedTarget = "DisconnectCall";
                });
            }
        }

        public CompositeNodeViewModel CompositeNode
        {
            get
            {
                return (DataContext as CompositeNodeViewModel);
            }
        }

        #region SelectedTarget

        public string SelectedTarget
        {
            get
            {
                return (string)GetValue(SelectedTargetProperty);
            }
            set
            {
                SetValue(SelectedTargetProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedTargetProperty =
            DependencyProperty.Register("SelectedTarget", typeof(string), typeof(TargetSelector), new FrameworkPropertyMetadata(new PropertyChangedCallback(PropertyChanged))
            {
                BindsTwoWayByDefault = true
            });

        #endregion

        private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TargetSelector instance = sender as TargetSelector;

            if (e.Property == SelectedTargetProperty)
            {
                instance.comboBox.DataContext = instance.CompositeNode.GraphModel.FindNodeByID((string)e.NewValue);
                if (e.NewValue != null && Regex.IsMatch(e.NewValue.ToString(), @"\d+"))
                {
                    if (instance.comboBox.DataContext != null)
                        instance.pickedNodeComboBoxItem.IsSelected = true;
                    else
                        instance.noneComboBoxItem.IsSelected = true;
                    return;
                }
                instance.SelectItemByTag((string)e.NewValue);
                instance.CompositeNode.GraphModel.IsSelectingTarget = false;
            }
        }

        private void SelectItemByTag(string tag)
        {
            foreach (ComboBoxItem item in comboBox.Items)
                if ((string)item.Tag == tag)
                {
                    item.IsSelected = true;
                    return;
                }
        }

        private void TargetSelector_NodePicked(UMSV.TreeNode pickedNode)
        {
            if (this.IsInSelectMode)
            {
                CompositeNode.GraphModel.IsSelectingTarget = false;
                SelectedTarget = pickedNode.GraphNode.StartNode;
                if (!pickedNodeComboBoxItem.IsSelected)
                    pickedNodeComboBoxItem.IsSelected = true;
            }
        }

        private bool IsInSelectMode
        {
            get
            {
                return comboBox.SelectedItem == selectorComboBoxItem;
            }
        }

        private void selectorComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            CompositeNode.GraphModel.IsSelectingTarget = true;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox.SelectedItem != pickedNodeComboBoxItem && comboBox.SelectedItem != selectorComboBoxItem)
            {
                SelectedTarget = (string)(comboBox.SelectedItem as ComboBoxItem).Tag;
            }
            else if (comboBox.SelectedItem == pickedNodeComboBoxItem)
            {
                CompositeNode.GraphModel.IsSelectingTarget = false;
            }
        }

    }
}
