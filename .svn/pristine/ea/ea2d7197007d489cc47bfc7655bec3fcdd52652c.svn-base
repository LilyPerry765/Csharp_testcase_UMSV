using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMSV.Schema;
using Pendar.Ums.Model;
using Folder.Converter;
using UMSV;
using Enterprise;

namespace UMSV
{
    public class TreeNode : TreeViewItem
    {
        public TreeNode(UserControl nodeUI)
            : this(nodeUI, null)
        {
        }

        public TreeNode(UserControl nodeUI, NodeGroup graphNode)
        {
            nodeInitInfo = new CompositeNodeInfo(nodeUI);
            // When node is loaded from xml
            if (graphNode != null)
                this.graphNode = graphNode;
            ContextMenuOpening += new ContextMenuEventHandler(TreeNode_ContextMenuOpening);
            PropertyUI = nodeUI;
            this.GraphNode.AdjustStartAndLastNodes(nodeInitInfo);
            Header = CreateHeader();
        }

        void TreeNode_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (ContextMenu != null && (e.Source as System.Windows.FrameworkElement).FindParent<TreeNode>() == this)
                this.ContextMenu.Items.Filter = new Predicate<object>(mi => CanViewMenuItem(mi));
        }

        public bool CanViewMenuItem(object menuItem)
        {
            bool canView = false;

            if (!(menuItem is Separator) && ((menuItem as MenuItem).Tag as MenuItemTagExtension).IsStartNodeSetterMenuItem)
                canView = this.Parent == null && !IsStartNode;
            else if (!(menuItem is Separator) && !((menuItem as MenuItem).Tag as MenuItemTagExtension).IsCompositeNodeMenuItem)
                return true;
            else if (this.NodeInitInfo.Attribute.ChildMode == ChildModes.MultiChild)
                canView = true;
            else if (this.NodeInitInfo.Attribute.ChildMode == ChildModes.SingleChild && Items.Count == 0)
                canView = true;

            return canView;
        }

        /// <summary>
        /// Gets or sets whether this node is the start node of Graph.
        /// </summary>
        public bool IsStartNode
        {
            get
            {
                return isStartNode;
            }
            set
            {
                isStartNode = value;
                foreach (Label label in (Header as Panel).Children.Cast<UIElement>().Where(e => e is Label))
                {
                    //  label.FontWeight = value ? FontWeights.Black : FontWeights.Normal;
                    label.FontStyle = value ? FontStyles.Italic : FontStyles.Normal;
                }
            }
        }
        private bool isStartNode;

        /// <summary>
        /// If graph node is not set by ctor (that means the node is being created for the fisrt time), 
        /// it will use the default graphNode of the composite node. Otherwise, if the node is being deserialized from graph file,
        /// the graphNode is retrieved from graph xml file and set by ctor.
        /// </summary>
        private NodeGroup graphNode;
        public NodeGroup GraphNode
        {
            get
            {
                if (graphNode == null)
                {
                    graphNode = nodeInitInfo.GraphNode;
                }
                return graphNode;
            }

        }

        private CompositeNodeInfo nodeInitInfo;
        public CompositeNodeInfo NodeInitInfo
        {
            get
            {
                return nodeInitInfo;
            }
        }

        private Label extendedHeaderLabel = new Label()
        {
            Padding = new Thickness(1),
            VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
        };

        /// <summary>
        /// Invokes the given action on this TreeNode and all its sub-nodes.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        public void CascadeAction(Action<TreeNode> action)
        {
            DoForAll(this, action);
        }

        private void DoForAll(TreeNode root, Action<TreeNode> action)
        {
            action(root);
            foreach (var item in root.Items)
            {
                DoForAll(item as TreeNode, action);
            }
        }

        /// <summary>
        /// Gets or sets whether or not this TreeNode has children.
        /// </summary>
        public bool IsChildless
        {
            get
            {
                return (bool)GetValue(IsChildlessProperty);
            }
            set
            {
                SetValue(IsChildlessProperty, value);
            }
        }

        public static readonly DependencyProperty IsChildlessProperty =
            DependencyProperty.Register("IsChildless", typeof(bool), typeof(TreeNode), new UIPropertyMetadata(true));

        private object CreateHeader()
        {
            Image icon = nodeInitInfo.Image;
            Label titleLabel = new Label()
            {
                VerticalContentAlignment = System.Windows.VerticalAlignment.Center,
                Padding = new Thickness(1),
                Margin = new Thickness(2, 0, 2, 0),
                DataContext = GraphNode
            };
            BindingOperations.SetBinding(titleLabel, Label.ContentProperty, new Binding("Description"));

            titleLabel.Height = icon.Width = icon.Height = extendedHeaderLabel.Height = 20;
            StackPanel panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };
            panel.Children.Add(icon);
            panel.Children.Add(extendedHeaderLabel);
            panel.Children.Add(titleLabel);
            return panel;
        }

        private void SetExtendedHeaderBinding(string formatString, object dataContext, params string[] paths)
        {
            extendedHeaderLabel.DataContext = dataContext;
            MultiBinding binding = new MultiBinding();
            binding.Converter = FormatConverter.Instance;
            binding.ConverterParameter = formatString;
            foreach (var path in paths)
                binding.Bindings.Add(new Binding(path));
            BindingOperations.SetBinding(extendedHeaderLabel, Label.ContentProperty, binding);
        }

        private void SetExtendedHeaderBindingWithSpecificConverter(IMultiValueConverter specificConverter, string specificConverterParameter, object dataContext, params string[] paths)
        {
            extendedHeaderLabel.DataContext = dataContext;
            MultiBinding binding = new MultiBinding();
            binding.Converter = specificConverter;
            binding.ConverterParameter = specificConverterParameter;
            foreach (var path in paths)
                binding.Bindings.Add(new Binding(path));
            BindingOperations.SetBinding(extendedHeaderLabel, Label.ContentProperty, binding);
        }

        public UserControl PropertyUI
        {
            get;
            protected set;
        }

        public override string ToString()
        {
            return GraphNode.Description;
        }

        #region Extension Manager

        /// <summary>
        /// Property ui for extended UI part (that is inherited from parent) of this node.
        /// </summary>
        public UserControl ExtendedUI
        {
            get;
            private set;
        }

        private string NextNodeResultValue(GetKeyNode hostNode)
        {
            foreach (char val in "1234567890*#")
                if (!hostNode.NodeResult.Any(nr => nr.Value == val.ToString()))
                    return val.ToString();
            // throw new UmsException("تعداد گره های اضافه شده به حداکثر رسيده است.");
            return null;
        }

        /// <summary>
        /// When called by a child node, attaches the extended UI to its property UI.
        /// </summary>
        /// <param name="childNode">GraphNode of the subscriber node.</param>
        public void AttachExtendedUI(TreeNode childNode)
        {
            if (GraphNode.Tag == "GetKey")
            {
                AttachGetKeyNodeResult(childNode);
            }
            else if (graphNode.Tag == "Invoke")
            {
                AttachInvokeNodeResult(childNode);
            }
            if (GraphNode.Tag == "TimeMatch")
            {
                AttachMatchTime(childNode);
            }
            else if (GraphNode.Tag == "CallerIDMatch")
            {
                AttachMatchCallerID(childNode);
            }
            else if (GraphNode.Tag == "DateMatch")
            {
                AttachMatchDate(childNode);
            }
        }

        private void AttachGetKeyNodeResult(TreeNode childNode)
        {
            try
            {
                NodeResult matchingNode = GraphNode.GetKeyNodes[0].NodeResult.FirstOrDefault(nr => nr.TargetNode == childNode.GraphNode.StartNode);
                if (matchingNode == null)
                {
                    matchingNode = new NodeResult() {
                        TargetNode = childNode.GraphNode.StartNode,
                        Value = NextNodeResultValue(GraphNode.GetKeyNodes[0])
                    };
                    GraphNode.GetKeyNodes[0].NodeResult.Add(matchingNode);
                }
                childNode.ExtendedUI = new UMSV.BasicNodes.GetKeyNodeResultView() {
                    DataContext = matchingNode
                };
                childNode.SetExtendedHeaderBinding("({0})", matchingNode, "Value");
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Sequence contains more than one matching element"))
                    Logger.WriteException("Error in AttachGetKeyNodeResult, more than one GetKeyNode result found, TargetNode:{0}", childNode.GraphNode.StartNode);
                else
                    Logger.Write(ex);
            }
        }


        private void AttachInvokeNodeResult(TreeNode childNode)
        {
            NodeResult matchingNode = GraphNode.InvokeNodes[0].NodeResult.FirstOrDefault(nr => nr.TargetNode == childNode.GraphNode.StartNode);
            if (matchingNode == null)
            {
                matchingNode = new NodeResult()
                {
                    TargetNode = childNode.GraphNode.StartNode,
                };
                GraphNode.InvokeNodes[0].NodeResult.Add(matchingNode);
            }
            childNode.ExtendedUI = new UMSV.BasicNodes.InvokeNodeResultView()
            {
                DataContext = matchingNode
            };
            childNode.SetExtendedHeaderBinding("({0})", matchingNode, "Value");
        }

        private void AttachMatchCallerID(TreeNode childNode)
        {
            SelectNodeMatchCallerID matchingNode = GraphNode.SelectNodes[0].MatchCallerIDs.FirstOrDefault(n => n.TargetNode == childNode.GraphNode.StartNode);
            if (matchingNode == null)
            {
                matchingNode = new SelectNodeMatchCallerID()
                    {
                        TargetNode = childNode.GraphNode.StartNode
                    };
                GraphNode.SelectNodes[0].MatchCallerIDs.Add(matchingNode);
            }
            childNode.ExtendedUI = new UMSV.BasicNodes.MatchCallerIDView()
            {
                DataContext = matchingNode
            };
            childNode.SetExtendedHeaderBinding("({0})", matchingNode, "Code");
        }

        private void AttachMatchTime(TreeNode childNode)
        {
            try
            {
                SelectNodeMatchTime matchingNode = GraphNode.SelectNodes[0].MatchTimes.FirstOrDefault(mt => mt.TargetNode == childNode.GraphNode.StartNode);
                if (matchingNode == null)
                {
                    DateTime t = DateTime.Today;
                    matchingNode = new SelectNodeMatchTime()
                    {
                        TargetNode = childNode.GraphNode.StartNode,
                        StartTime = new DateTime(t.Year, t.Month, t.Day, 0, 0, 0),
                        EndTime = new DateTime(t.Year, t.Month, t.Day, 23, 59, 59)
                    };
                    GraphNode.SelectNodes[0].MatchTimes.Add(matchingNode);
                }
                childNode.ExtendedUI = new UMSV.BasicNodes.MatchTimeView()
                {
                    DataContext = matchingNode
                };
                childNode.SetExtendedHeaderBinding("({1:HH:mm} - {0:HH:mm})", matchingNode, "StartTime", "EndTime");

            }
            catch (Exception ex)
            {
                Enterprise.Logger.Write(ex);
            }
        }

        private void AttachMatchDate(TreeNode childNode)
        {
            try
            {
                SelectNodeMatchDate matchingNode = GraphNode.SelectNodes[0].MatchDates.FirstOrDefault(mt => mt.TargetNode == childNode.GraphNode.StartNode);
                if (matchingNode == null)
                {
                    DateTime t = DateTime.Today;
                    matchingNode = new SelectNodeMatchDate()
                    {
                        TargetNode = childNode.GraphNode.StartNode,
                        StartDate = t.Date,
                        EndDate = t.AddDays(1).Date
                    };
                    GraphNode.SelectNodes[0].MatchDates.Add(matchingNode);
                }
                childNode.ExtendedUI = new UMSV.BasicNodes.MatchDateView()
                {
                    DataContext = matchingNode
                };
                childNode.SetExtendedHeaderBindingWithSpecificConverter((IMultiValueConverter)Pendar.Ums.Model.Converters.PersianDateMultiValueConverter.Instance,
                    "({1} - {0})", matchingNode, "StartDate", "EndDate");

            }
            catch (Exception ex)
            {
                Enterprise.Logger.Write(ex);
            }
        }



        //private void AttachPlayNodeResult(TreeNode childNode)
        //{
        //    // The target node is "Bye" when the playnode doesn't have any sub-nodes; therefore, once it is assigned a sub-node, its targetnode must be set to the GetKeyNode.
        //    if (GraphNode.PlayNodes[0].TargetNode != GraphNode.GetKeyNodes[0].ID)
        //        GraphNode.PlayNodes[0].TargetNode = GraphNode.GetKeyNodes[0].ID;

        //    NodeResult matchingNode = GraphNode.GetKeyNodes[0].NodeResult.SingleOrDefault(nr => nr.TargetNode == childNode.GraphNode.StartNode);
        //    if (matchingNode == null)
        //    {
        //        matchingNode = new NodeResult()
        //        {
        //            TargetNode = childNode.GraphNode.StartNode,
        //            Value = NextNodeResultValue(GraphNode.GetKeyNodes[0])
        //        };
        //        GraphNode.GetKeyNodes[0].NodeResult.Add(matchingNode);
        //    }
        //    childNode.ExtendedUI = new NodeResultView()
        //    {
        //        DataContext = matchingNode
        //    };
        //    childNode.SetExtendedHeaderBinding("({0})", matchingNode, "Value");
        //}

        /// <summary>
        /// When a child node is being removed, calls this method on its parent, to remove its coresponding "selector child" from its parent "selector node".
        /// </summary>
        /// <param name="extendedDataContext">The coresponding "selector child" of the child node which is located in the dataContext of extendedUI of child node.</param>
        internal void DetachExtendedUI(object extendedDataContext)
        {
            if (extendedDataContext is NodeResult)
            {
                if (GraphNode.GetKeyNodes.Any() && GraphNode.GetKeyNodes[0].NodeResult.Count > 0)
                    GraphNode.GetKeyNodes[0].NodeResult.Remove(extendedDataContext as NodeResult);
                else if (GraphNode.InvokeNodes.Any() && GraphNode.InvokeNodes[0].NodeResult.Count > 0)
                    GraphNode.InvokeNodes[0].NodeResult.Remove(extendedDataContext as NodeResult);
                else if (GraphNode.DivertNodes.Any() && GraphNode.DivertNodes[0].NodeResult.Count > 0)
                    GraphNode.DivertNodes[0].NodeResult.Remove(extendedDataContext as NodeResult);
            }
            else if (extendedDataContext is SelectNodeMatchTime)
            {
                GraphNode.SelectNodes[0].MatchTimes.Remove(extendedDataContext as SelectNodeMatchTime);
            }
            else if (extendedDataContext is SelectNodeMatchCallerID)
            {
                GraphNode.SelectNodes[0].MatchCallerIDs.Remove(extendedDataContext as SelectNodeMatchCallerID);
            }
            else if (extendedDataContext is SelectNodeMatchDate)
            {
                GraphNode.SelectNodes[0].MatchDates.Remove(extendedDataContext as SelectNodeMatchDate);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region Offset Management

        public int MaxID
        {
            get
            {
                return GetMaxID(GraphNode);
            }
        }


        private int GetMaxID(NodeGroup root)
        {
            int max = 0;
            foreach (var item in root.Items)
            {
                if (item is NodeGroup)
                    max = Math.Max(max, GetMaxID(item as NodeGroup));
                else if (item is Node)
                {
                    string nodeID = (item as Node).ID;
                    if (nodeID.ToInt().HasValue)
                        max = Math.Max(max, nodeID.ToInt().Value);
                }
            }
            return max;
        }


        #endregion
    }
}
