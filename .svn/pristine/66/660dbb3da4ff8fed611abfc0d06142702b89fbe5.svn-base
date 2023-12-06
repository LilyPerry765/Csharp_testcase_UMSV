using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Input;
using System.Windows.Controls;
using UMSV.Schema;
using Folder.Commands;
using System.Windows;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using Pendar.Ums.Model;
using System.Reflection;
using Folder;
using System.Windows.Media.Imaging;
using UMSV;
using Enterprise;

namespace UMSV.ViewModels
{
    public class GraphViewModel : ViewModelBase, IEditableForm
    {

        #region Events

        public event Action<TreeNode> NodePicked;
        public event Action<TreeNode> NodeRemoving;

        #endregion

        UMSV.Graph dbGraph;
        bool newGraph = false;

        SaveFileDialog dlg = new SaveFileDialog()
        {
            DefaultExt = "xml",
            Filter = "*.xml|*.xml",
            FileName = "Graph.xml"
        };
        OpenFileDialog dlgOpen = new OpenFileDialog()
        {
            DefaultExt = "xml",
            Filter = "*.xml|*.xml",
            FileName = "Graph.xml"
        };
        #region Constructors

        public GraphViewModel()
        {
            Nodes = new ObservableCollection<TreeNode>(new List<TreeNode>());
        }

        public GraphViewModel(Guid? graphID)
            : this()
        {
            if (!graphID.HasValue || graphID == Guid.Empty)
            {
                newGraph = true;
                this.dbGraph = new UMSV.Graph()
                {
                    ID = Guid.NewGuid(),
                    Name = "درختواره جدید",
                };
                dc.Graphs.InsertOnSubmit(this.dbGraph);
            }
            else
                this.dbGraph = dc.Graphs.FirstOrDefault(g => g.ID == graphID);

            if (dbGraph.Data == null)
            {
                Graph = CreateGraph();
            }
            else
                Graph = UMSV.Schema.Graph.Deserialize(dbGraph.Data.ToString());

            if (Graph.Description == null)
                Graph.Description = dbGraph.Name;
        }

        #endregion

        #region Offsetting

        private void ApplyOffset(object rootNode)
        {
            ApplyOffset(rootNode, GetNextID());
        }

        private static void ApplyOffset(object rootNode, int offset)
        {
            string[] offsetableProperties = { "TargetNode", "ID", "TimeoutNode", "StartNode", "LastNode", "MaxDigitsNode" };
            foreach (string prop in offsetableProperties)
                ApplyOffsetToProperty(rootNode, prop, offset);
            if (rootNode is NodeGroup)
                foreach (var item in (rootNode as NodeGroup).Items)
                    ApplyOffset(item, offset);
            else if (rootNode is GetKeyNode)
                foreach (var item in (rootNode as GetKeyNode).NodeResult)
                    ApplyOffset(item, offset);
            else if (rootNode is InvokeNode)
                foreach (var item in (rootNode as InvokeNode).NodeResult)
                    ApplyOffset(item, offset);
            else if (rootNode is SelectNode)
            {
                foreach (var item in (rootNode as SelectNode).MatchCallerIDs)
                    ApplyOffset(item, offset);
                foreach (var item in (rootNode as SelectNode).MatchTimes)
                    ApplyOffset(item, offset);
                foreach (var item in (rootNode as SelectNode).MatchDates)
                    ApplyOffset(item, offset);
            }
        }

        private static void ApplyOffsetToProperty(object obj, string propertyName, int offset)
        {
            PropertyInfo prop = obj.GetType().GetProperty(propertyName);
            if (prop != null)
            {
                string stringValue = (string)prop.GetValue(obj, null);
                if (stringValue.ToInt().HasValue)
                {
                    stringValue = string.Format("#{0}", stringValue.ToInt().Value + offset);
                    prop.SetValue(obj, stringValue, null);
                }
            }
        }

        #endregion

        #region Node Management

        public ObservableCollection<TreeNode> Nodes
        {
            get;
            set;
        }

        public TreeNode SelectedNode
        {
            get
            {
                return FindSelectedNode(Nodes);
            }
            set
            {
                if (value == null)
                {
                    if (SelectedNode != null)
                        SelectedNode.IsSelected = false;
                }
                else
                {
                    value.IsSelected = true;
                    value.BringIntoView();
                }
                // There is no need to call SendPropertyChanged("SelectedNode") here, because this method is called in "Selected" event handler of the TreeNode.
            }
        }

        private TreeNode FindSelectedNode(ICollection rootNodes)
        {
            foreach (TreeNode item in rootNodes)
            {
                if (item.IsSelected)
                    return item;
                TreeNode selectedItem = FindSelectedNode(item.Items);
                if (selectedItem != null)
                    return selectedItem;
            }
            return null;
        }

        /// <summary>
        /// Finds a TreeNode in the graph according to its GraphNode.StartNode.
        /// </summary>
        /// <param name="startNodeID">The ID of the StartNode of the GraphNode of the TreeNode that is being searched.</param>
        public TreeNode FindNodeByID(string startNodeID)
        {
            return FindNodeByID(Nodes, startNodeID);
        }

        private TreeNode FindNodeByID(ICollection rootNodes, string nodeID)
        {
            foreach (TreeNode item in rootNodes)
            {
                if (item.GraphNode.StartNode == nodeID)
                    return item;
                TreeNode foundItem = FindNodeByID(item.Items, nodeID);
                if (foundItem != null)
                    return foundItem;
            }
            return null;
        }

        private bool RemoveFromTree(TreeNode nodeToRemove)
        {
            if (nodeToRemove != null)
            {
                OnNodeRemoving(nodeToRemove);
                // When the parent is not a TreeNode, it's null; so whenever the parent is not null, it is a TreeNode.
                TreeNode parent = nodeToRemove.Parent as TreeNode;
                if (parent == null)
                {
                    Graph.Items.Remove(nodeToRemove.GraphNode);
                    Nodes.Remove(nodeToRemove);
                }
                else
                {
                    parent.GraphNode.Items.Remove(nodeToRemove.GraphNode);
                    (parent.PropertyUI.DataContext as CompositeNodeViewModel).IsTargetLocked = false;
                    if (nodeToRemove.ExtendedUI != null)
                        parent.DetachExtendedUI(nodeToRemove.ExtendedUI.DataContext);
                    parent.Items.Remove(nodeToRemove);
                    parent.IsChildless = parent.Items.Count == 0;
                }
                return true;
            }
            return false;
        }

        private void OnNodeRemoving(TreeNode nodeToRemove)
        {
            if (nodeToRemove.IsStartNode)
            {
                SetAsStartNode(nodeToRemove, false);
            }
            if (NodeRemoving != null)
            {
                NodeRemoving(nodeToRemove);
            }
            nodeToRemove.CascadeAction(tn =>
            {
                (tn.PropertyUI.DataContext as CompositeNodeViewModel).OnRemoving();
            });
        }

        private void AddToTree(TreeNode newTreeNode, TreeNode parentNode)
        {
            newTreeNode.PropertyUI.DataContext = new CompositeNodeViewModel(newTreeNode, this);
            if (!IsReadOnly)
                newTreeNode.ContextMenu = NodeContextMenu;
            newTreeNode.Selected += treeNode_Selected;
            newTreeNode.PreviewMouseDown += treeNode_PreviewMouseDown;
            // newTreeNode.MouseDown += treeNode_MouseDown;
            if (parentNode == null)
            {
                // Adding node to tree root
                AddToGraph(newTreeNode, Graph);
                Nodes.Add(newTreeNode);
            }
            else
            {
                AddToGraph(newTreeNode, parentNode.GraphNode);
                parentNode.Items.Add(newTreeNode);
                parentNode.IsChildless = false;

                if (parentNode.NodeInitInfo.Attribute.ChildMode == ChildModes.SingleChild)
                {
                    parentNode.GraphNode.SetTarget(newTreeNode.GraphNode.StartNode);
                    (parentNode.PropertyUI.DataContext as CompositeNodeViewModel).IsTargetLocked = true;
                }
                parentNode.AttachExtendedUI(newTreeNode);
            }
            if (!isLoading)
            {
                newTreeNode.BringIntoView();
                if (newTreeNode.Focus())
                    newTreeNode.IsSelected = true;
            }
        }


        private void AddToGraph(TreeNode childNode, GraphGroup parent)
        {
            if (!parent.Items.Contains(childNode.GraphNode))
            {
                ApplyOffset(childNode.GraphNode);
                // When loading the graph from file, graphNodes are already present in the graph, so in this case re-adding nodeGroup to graph results in duplicate nodes.
                parent.Items.Add(childNode.GraphNode);
            }
            if (Graph.StartNode == "Bye")
            {
                SetAsStartNode(childNode);
            }
        }

        private void SetAsStartNode(TreeNode treeNode, bool isStartNode = true)
        {
            if (isStartNode)
            {
                TreeNode prevStartNode = FindNodeByID(Graph.StartNode);
                if (prevStartNode != null)
                    prevStartNode.IsStartNode = false;
                Graph.StartNode = treeNode.GraphNode.StartNode;
            }
            else
            {
                Graph.StartNode = "Bye";
            }
            treeNode.IsStartNode = isStartNode;
        }

        #region TreeNode Event Handlers

        //void treeNode_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (e.ChangedButton == MouseButton.Right)
        //    {
        //        //(sender as TreeNode).IsSelected = true;
        //       // e.Handled = true;
        //    }
        //}

        void treeNode_Selected(object sender, RoutedEventArgs e)
        {
            SendPropertyChanged("SelectedNode");
            e.Handled = true;
        }

        void treeNode_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton == MouseButton.Left && IsSelectingTarget)
            {
                e.Handled = true;
                TreeNode pickedNode = (e.Source as FrameworkElement).FindParent<TreeNode>();
                if (pickedNode == SelectedNode)
                {
                    Folder.MessageBox.ShowError("گره جاری نمی تواند به عنوان گره بعدی انتخاب شود.\nلطفا گره ديگری را انتخاب نماييد.");
                }
                else
                {
                    if (NodePicked != null)
                        NodePicked(pickedNode);
                    IsSelectingTarget = false;
                }
            }
            else
            {
                (sender as TreeNode).IsSelected = true;
            }
        }

        #endregion

        #endregion

        #region IsSelectingTarget

        private bool isSelectingTarget;
        public bool IsSelectingTarget
        {
            get
            {
                return isSelectingTarget;
            }
            set
            {
                isSelectingTarget = value;
                Cursor = isSelectingTarget ? Cursors.Hand : Cursors.Arrow;
                SendPropertyChanged("IsSelectingTarget");
            }
        }

        #endregion

        #region Graph Management

        public static UMSV.Schema.Graph CreateGraph()
        {
            UMSV.Schema.Graph graph = new UMSV.Schema.Graph();
            graph.StartNode = "Bye";
            graph.Items.Add(new PlayNode()
            {
                ID = "Bye",
                TargetNode = "DisconnectCall",
            });
            graph.Items.Add(new InvokeNode()
            {
                ID = "DisconnectCall",
                Function = "DisconnectCall"
            });
            graph.PlayNodes[0].Voice.Add(new UMSV.Schema.Voice()
            {
                Name = "Bye",
                Type = 0,
                Group = 0
            });
            return graph;
        }

        private UMSV.Schema.Graph graph;
        public UMSV.Schema.Graph Graph
        {
            get
            {
                return graph;
            }
            set
            {
                Cursor prevCursor = Cursor;
                Cursor = Cursors.Wait;
                System.Windows.Forms.Application.DoEvents();
                if (Nodes.Count > 0)
                {
                    while (Nodes.Count > 0)
                        RemoveFromTree(Nodes[0]);
                    SendPropertyChanged("SelectedNode");
                }
                isLoading = true;
                graph = value;
                LoadGraphOnTree(graph, null);
                var startNode = FindNodeByID(Graph.StartNode);
                if (startNode != null)
                {
                    startNode.IsStartNode = true;
                }
                isLoading = false;
                if (Nodes.Any())
                    SelectedNode = Nodes[0];
                Cursor = prevCursor;
            }
        }

        private void LoadGraphOnTree(GraphGroup rootGraphGroup, TreeNode parentNode)
        {
            if (rootGraphGroup != null)
            {
                foreach (var nodeGroup in rootGraphGroup.NodeGroups)
                {
                    UpgradeOldNodes(nodeGroup);
                    CompositeNodeInfo nodeInfo = CompositeNodeInfo.FindByTag(nodeGroup.Tag);
                    TreeNode newTreeNode = new TreeNode(nodeInfo.UI, nodeGroup);
                    AddToTree(newTreeNode, parentNode);
                    LoadGraphOnTree(nodeGroup, newTreeNode);
                }
                //if (parentNode != null)
                //    parentNode.IsExpanded = ExpandNodesByDefault;
            }
        }

        private void UpgradeOldNodes(NodeGroup nodeGroup)
        {
            if (nodeGroup.Tag == "Play" && nodeGroup.GetKeyNodes.Any())
                UpgradePlayNode(nodeGroup);

            if (nodeGroup.Tag == "UserData" && nodeGroup.InvokeNodes.Length == 1)
                UpgradeUserDataNode(nodeGroup);
        }

        private void UpgradeUserDataNode(NodeGroup oldUserDataNode)
        {
            CompositeNodeInfo userDataNodeInfo = CompositeNodeInfo.FindByTag("UserData");
            NodeGroup virginNode = userDataNodeInfo.GraphNode;
            ApplyOffset(virginNode);
            oldUserDataNode.Items.Insert(2, virginNode.InvokeNodes[0]);
            oldUserDataNode.Items.Insert(3, virginNode.PlayNodes[1]);
            oldUserDataNode.Items.Insert(4, virginNode.GetKeyNodes[1]);
            virginNode.Items.Clear();
            oldUserDataNode.GetKeyNodes[0].TimeoutNode = oldUserDataNode.InvokeNodes[0].ID;
            oldUserDataNode.GetKeyNodes[0].MaxDigitsNode = oldUserDataNode.InvokeNodes[0].ID;
            oldUserDataNode.GetKeyNodes[1].NodeResult[0].TargetNode = oldUserDataNode.InvokeNodes[1].ID;
            oldUserDataNode.GetKeyNodes[1].NodeResult[1].TargetNode = oldUserDataNode.PlayNodes[0].ID;
            if (!oldUserDataNode.PlayNodes[0].Voice.Any())
                oldUserDataNode.PlayNodes[0].Voice.Add(new UMSV.Schema.Voice());
        }

        private void UpgradePlayNode(NodeGroup oldPlayNode)
        {
            CompositeNodeInfo getKeyNodeInfo = CompositeNodeInfo.FindByTag("GetKey");

            NodeGroup getKeyCompositeNode = getKeyNodeInfo.GraphNode;
            getKeyCompositeNode.Items.Remove(getKeyCompositeNode.GetKeyNodes[0]);
            getKeyCompositeNode.Items.Add(oldPlayNode.GetKeyNodes[0]);
            oldPlayNode.Items.Remove(oldPlayNode.GetKeyNodes[0]);
            foreach (NodeGroup compositeNodeFromOldPlayNode in oldPlayNode.NodeGroups)
            {
                getKeyCompositeNode.Items.Add(compositeNodeFromOldPlayNode);
                oldPlayNode.Items.Remove(compositeNodeFromOldPlayNode);
            }
            oldPlayNode.Items.Add(getKeyCompositeNode);

        }

        private int GetMaxTreeID(ICollection rootItems)
        {
            int max = 0;
            foreach (TreeNode n in rootItems)
            {
                max = Math.Max(max, n.MaxID);
                if (n.Items != null && n.Items.Count > 0)
                    max = Math.Max(max, GetMaxTreeID(n.Items));
            }
            return max;
        }

        public int GetNextID()
        {
            return GetMaxTreeID(Nodes) + 1;
        }

        #endregion

        #region Validation

        public bool Validate()
        {
            return Validate(Nodes);
        }

        private bool Validate(ICollection rootNodes)
        {
            foreach (TreeNode item in rootNodes)
            {
                IValidatable nodeUI = item.PropertyUI as IValidatable;
                ValidationResult validationResult;
                item.Focus();
                item.IsSelected = true;
                item.IsExpanded = true;
                if (nodeUI != null && !(validationResult = nodeUI.Validate()).IsValid)
                {
                    if (validationResult.ErrorContent != null)
                        Folder.MessageBox.ShowError(validationResult.ErrorContent.ToString());
                    return false;
                }
                if (item.Items != null && item.Items.Count > 0)
                    if (!Validate(item.Items))
                        return false;
            }
            return true;
        }

        #endregion

        #region IO

        private void WriteVoice(string id)
        {
            try
            {
                if (id.Length < 20)
                    return;
                using (UmsDataContext umsDC = new UmsDataContext())
                {
                    byte[] data = umsDC.Voices.SingleOrDefault(p => p.ID == (new Guid(id))).Data.ToArray();
                    if (data == null || data.Length == 0)
                        return;

                    byte[] wavData = Folder.Audio.AudioUtility.ConvertMsg2Wave(data);

                    string path = Path.GetDirectoryName(dlg.FileName);
                    path = Path.Combine(path, id + ".wav");
                    System.IO.File.WriteAllBytes(path, wavData);
                }
            }
            catch (Exception)
            {
                Logger.Write(LogType.Error, "Error exporting voice ID:{0}", id);
            }
        }
        private void ExportVoice(GraphGroup group)
        {
            foreach (object item in group.Item)
            {
                if (item.GetType() == typeof(PlayNode) && (((PlayNode)item).ID != "Bye" && ((PlayNode)item).ID != "Disconnect"))
                {
                    foreach (Schema.Voice voice in ((PlayNode)item).Voice)
                    {
                        if (!string.IsNullOrEmpty(voice.ID))
                        {
                            WriteVoice(voice.ID);
                        }
                    }
                }
                else if (item.GetType() == typeof(InvokeNode))
                {
                    if (((InvokeNode)item).Function == "PlayPredefinedVoice" && ((InvokeNode)item).Arg.Count == 2)
                    {
                        string[] voiceIds = ((InvokeNode)item).Arg[1].Value.Split(",".ToCharArray(), 15, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string voiceID in voiceIds)
                        {
                            WriteVoice(voiceID);
                        }
                    }
                }
                else if (item.GetType() == typeof(NodeGroup))
                {
                    ExportVoice((GraphGroup)item);
                }
            }
        }
        private void Export()
        {

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    string xml = Graph.Serialize(true);
                    System.IO.File.WriteAllText(dlg.FileName, xml, System.Text.Encoding.Unicode);

                    ExportVoice(graph);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
        private void Import()
        {
            try
            {
                if (dlgOpen.ShowDialog() == true)
                {
                    using (StreamReader streamReader = System.IO.File.OpenText(dlgOpen.FileName))
                    {
                        Graph = UMSV.Schema.Graph.Deserialize(streamReader.ReadToEnd());
                    }

                    foreach (var file in System.IO.Directory.GetFiles(System.IO.Path.GetDirectoryName(dlgOpen.FileName), "*.wav"))
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        Guid voiceID;
                        if (Guid.TryParse(fileName, out voiceID))
                        {
                            var voiceExist = dc.Voices.FirstOrDefault(v => v.ID == voiceID);
                            if (voiceExist == null)
                            {
                                UMSV.Voice voice = new UMSV.Voice()
                                {
                                    ID = voiceID,
                                    VoiceGroup = (byte)UMSV.VoiceGroup.CustomerCare,
                                    Data = Folder.Audio.AudioUtility.ConvertWave2Msg(System.IO.File.ReadAllBytes(file))
                                };
                                dc.Voices.InsertOnSubmit(voice);
                            }
                            else
                            {
                                voiceExist.Data = Folder.Audio.AudioUtility.ConvertWave2Msg(System.IO.File.ReadAllBytes(file));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteException(ex.Message);
            }

        }

        #endregion


        public string GraphName
        {
            get
            {
                return dbGraph.Name;
            }
            set
            {
                dbGraph.Name = value;
            }
        }

        #region Menu Management

        private IEnumerable rootMenuItems;
        public IEnumerable RootMenuItems
        {
            get
            {
                if (rootMenuItems == null)
                {
                    rootMenuItems = CreateMenu(true);
                }
                return rootMenuItems;
            }
        }

        private ContextMenu nodeContextMenu;
        public ContextMenu NodeContextMenu
        {
            get
            {
                if (nodeContextMenu == null)
                {
                    nodeContextMenu = new ContextMenu()
                    {
                        ItemsSource = CreateMenu(false),
                        FontFamily = new System.Windows.Media.FontFamily("Tahoma")
                    };
                }
                return nodeContextMenu;
            }
        }

        /// <param name="isRootMenu">Determines whether this is the root menu of the tree or the menu for TreeNodes.</param>
        private IEnumerable CreateMenu(bool isRootMenu)
        {
            List<object> menuCollection = new List<object>();
            IList currentMenuCollection;

            foreach (var subMenuCompositeNodes in CompositeNodeInfo.GetAll(true).GroupBy(c => c.Attribute.SubMenu))
            {
                // If this group shoul be placed in a sub-menu (its SubMenu name is not null):
                if (subMenuCompositeNodes.Key != null)
                {
                    menuCollection.Add(new Separator());
                    currentMenuCollection = AddMenuItem(menuCollection, subMenuCompositeNodes.Key, null, null, true).Items;
                }
                else
                    currentMenuCollection = menuCollection;

                int groupSequence = 0;
                foreach (var compositeNodeGroup in subMenuCompositeNodes.GroupBy(p => p.Attribute.GroupIndex).OrderBy(g => g.Key))
                {
                    if (groupSequence++ > 0)
                        currentMenuCollection.Add(new Separator());

                    foreach (var compositeNodeInfo in compositeNodeGroup.OrderBy(p => p.Attribute.Index))
                    {
                        CompositeNodeInfo compositeNodeInfoThisItem = compositeNodeInfo;
                        AddMenuItem(currentMenuCollection, compositeNodeInfoThisItem.Title, compositeNodeInfoThisItem.Image, delegate()
                        {
                            AddToTree(new TreeNode(compositeNodeInfoThisItem.NewUiInstance()), isRootMenu ? null : SelectedNode);
                        }, true);
                    }
                }
            }

            if (isRootMenu)
            {
                menuCollection.Add(new Separator());
                AddMenuItem(menuCollection, "ذخيره", MenuIcon("save.png"), Export);
                AddMenuItem(menuCollection, "بازيابی", MenuIcon("open.png"), Import);
            }
            else
            {
                menuCollection.Add(new Separator());
                AddMenuItem(menuCollection, "انتخاب به عنوان گره شروع", MenuIcon("tick16.png"), delegate()
                {
                    SetAsStartNode(SelectedNode);
                }, isStartNodeSetter: true);

                AddMenuItem(menuCollection, "حذف گره", MenuIcon("delete16.png"), delegate()
                {
                    if (Folder.MessageBox.ShowQuestion("گره \"{0}\" حذف شود؟", SelectedNode.GraphNode.Description) == MessageBoxResult.Yes)
                    {
                        if (RemoveFromTree(SelectedNode))
                            SendPropertyChanged("SelectedNode");

                    }
                });
            }
            return menuCollection;
        }

        private Image MenuIcon(string path)
        {
            return new Image()
            {
                Source = new BitmapImage(new Uri("\\Images\\" + path, UriKind.RelativeOrAbsolute)),
                Height = 16,
                Width = 16
            };
        }

        private MenuItem AddMenuItem(IList container, string header, Image icon, Action action, bool isCompositeNodeMenuItem = false, bool isStartNodeSetter = false)
        {
            MenuItem addingMenuItem;
            container.Add(addingMenuItem = new MenuItem()
            {
                Header = header,
                Tag = new MenuItemTagExtension()
                {
                    IsCompositeNodeMenuItem = isCompositeNodeMenuItem,
                    IsStartNodeSetterMenuItem = isStartNodeSetter
                }
            });
            if (icon != null)
            {
                icon.Height = icon.Width = 16;
                addingMenuItem.Icon = icon;
            }
            if (action != null)
                addingMenuItem.Command = new DelegateCommand(action);
            return addingMenuItem;
        }

        #endregion

        /// <summary>
        /// Determines whether the graph is being loaded.
        /// </summary>
        private bool isLoading;
        public bool IsReadOnly
        {
            get;
            set;
        }

        UmsDataContext dc = new UmsDataContext();

        #region IEditableForm Members

        public bool Save()
        {
            if (Validate())
            {
                dbGraph.Data = System.Xml.Linq.XElement.Parse(Graph.Serialize());
                dc.SubmitChanges();
                if (newGraph)
                {
                    using (FolderDataContext fdc = new FolderDataContext())
                    {
                        if (!fdc.Roles.Any(r => r.ID == dbGraph.ID))
                        {
                            fdc.Roles.InsertOnSubmit(new Role()
                            {
                                ID = dbGraph.ID,
                                Name = dbGraph.Name,
                                ParentID = Constants.Role_GraphAccess,
                                Type = Role.RoleType_Simple
                            });
                            fdc.SubmitChanges();
                        }
                    }

                    User.Current.ReloadAllRoles();
                }

                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
