using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMSV.ViewModels;
using UMSV.Schema;

namespace Pendar.Ums.CompositeNodes.UserControls
{
    public partial class SelectorGraph : UserControl
    {
        private GraphViewModel graphModel = new GraphViewModel()
        {
            IsReadOnly = true,
        };

        public SelectorGraph()
        {
            InitializeComponent();
        }

        void graphModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedNode":
                    SelectedNodeID = graphModel.SelectedNode == null ? null : graphModel.SelectedNode.GraphNode.StartNode;
                    break;
                default:
                    break;
            }
        }

        #region Dependency Properties

        public string GraphID
        {
            get
            {
                return (string)GetValue(GraphIDProperty);
            }
            set
            {
                SetValue(GraphIDProperty, value);
            }
        }

        public static readonly DependencyProperty GraphIDProperty =
            DependencyProperty.Register("GraphID", typeof(string), typeof(SelectorGraph), new UIPropertyMetadata(PropertyChanged));

        public string SelectedNodeID
        {
            get
            {
                return (string)GetValue(SelectedNodeIDProperty);
            }
            set
            {
                SetValue(SelectedNodeIDProperty, value);
            }
        }

        public static readonly DependencyProperty SelectedNodeIDProperty =
            DependencyProperty.Register("SelectedNodeID", typeof(string), typeof(SelectorGraph), new FrameworkPropertyMetadata(PropertyChanged)
            {
                BindsTwoWayByDefault = true
            });

        #endregion


        private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            SelectorGraph me = sender as SelectorGraph;
            if (e.Property == GraphIDProperty)
            {
                if (me.IsVisible)
                    me.graphModel.Graph = LoadGraph(e.NewValue as string);
            }
            else if (e.Property == SelectedNodeIDProperty)
            {
                if (me.graphModel.Graph != null)
                    me.SetGraphSelectedNode(e.NewValue as string);
            }
        }

        private void SetGraphSelectedNode(string nodeID)
        {
            graphModel.SelectedNode = graphModel.FindNodeByID(nodeID);
            if (graphModel.SelectedNode != null)
            {
                var parent = graphModel.SelectedNode.Parent as UMSV.TreeNode;
                while (parent != null)
                {
                    parent.IsExpanded = true;
                    parent = parent.Parent as UMSV.TreeNode;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (graphModel.Graph == null && !string.IsNullOrEmpty(GraphID))
            {
                graphModel.Graph = LoadGraph(GraphID);
                SetGraphSelectedNode(SelectedNodeID);
                graphTreeView.DataContext = graphModel;
                graphModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(graphModel_PropertyChanged);
            }
        }

        private static Graph LoadGraph(string id)
        {
            UMSV.UmsDataContext db = new UMSV.UmsDataContext();
            var dbGraph = db.Graphs.FirstOrDefault(g => g.ID.ToString() == id);
            if (dbGraph != null)
                return Graph.Deserialize(dbGraph.Data.ToString());
            else
                return null;
        }





    }
}
