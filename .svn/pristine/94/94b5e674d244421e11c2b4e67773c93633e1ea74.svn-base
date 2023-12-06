using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using Enterprise;
using Pendar.Ums.Model;
using System.Linq;

namespace UMSV.Schema
{
    partial class Node : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, ID= {1}", GetType().Name, ID);
        }

        public NodeType Type
        {
            get
            {
                switch (this.GetType().Name)
                {
                    case "PlayNode":
                        return NodeType.Play;
                    case "GetKeyNode":
                        return NodeType.GetKey;
                    case "InvokeNode":
                        return NodeType.Invoke;

                    case "RecordNode":
                        return NodeType.Record;

                    case "JumpNode":
                        return NodeType.Jump;

                    case "DivertNode":
                        return NodeType.Divert;

                    case "FaxNode":
                        return NodeType.Fax;

                    case "SelectNode":
                        return NodeType.Select;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public SelectNode AsSelectNode
        {
            get
            {
                return this as SelectNode;
            }
        }

        public PlayNode AsPlayNode
        {
            get
            {
                return this as PlayNode;
            }
        }

        public JumpNode AsJumpNode
        {
            get
            {
                return this as JumpNode;
            }
        }

        public GetKeyNode AsGetKeyNode
        {
            get
            {
                return this as GetKeyNode;
            }
        }

        public DivertNode AsDivertNode
        {
            get
            {
                return this as DivertNode;
            }
        }

        public FaxNode AsFaxNode
        {
            get
            {
                return this as FaxNode;
            }
        }

        public InvokeNode AsInvokeNode
        {
            get
            {
                return this as InvokeNode;
            }
        }

        public RecordNode AsRecordNode
        {
            get
            {
                return this as RecordNode;
            }
        }

        public WithTimerNode AsWithTimerNode
        {
            get
            {
                return this as WithTimerNode;
            }
        }

        public object HasTimeout
        {
            get
            {
                return this.GetType().IsSubclassOf(typeof(WithTimerNode)) && this.AsWithTimerNode._TimeoutSpecified;
            }
        }
    }

    partial class NodeGroup
    {
        public Node GetLastNode()
        {
            foreach (var item in Items)
            {
                Node node = item as Node;
                if (node != null && node.ID == this.LastNode)
                    return node;
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Description);
        }

        public void SetTarget(string target)
        {
            Node lastNode = GetLastNode();
            lastNode.SetPropertyValue("TargetNode", target);
        }

        /// <summary>
        /// Old ums graphs didn't have StartNode & LastNode, so when deserializing graph from xml, these properties must be retrieved from composite node init info.
        /// </summary>
        /// <param name="graphNode">The composite node for which to set start and last nodes</param>
        public void AdjustStartAndLastNodes(CompositeNodeInfo nodeInitInfo)
        {
            if (nodeInitInfo.GraphNode.StartNode != null)
            {
                if (nodeInitInfo.Attribute.Tag == "Invoke")
                {
                    if (this.GetKeyNodes.Any())
                    {
                        this.StartNode = GetKeyNodes[0].ID;
                    }
                    else if (this.RecordNodes.Any())
                    {
                        this.StartNode = RecordNodes[0].ID;
                    }
                    else
                    {
                        this.StartNode = InvokeNodes[0].ID;
                    }
                }
                else
                {
                    string startNodePath = nodeInitInfo.GraphNode.StartNode;
                    this.StartNode = this.GetNodeByPath(startNodePath).ID;
                }
            }
            if (nodeInitInfo.GraphNode.LastNode != null)
            {
                string lastNodePath = nodeInitInfo.GraphNode.LastNode;
                this.LastNode = this.GetNodeByPath(lastNodePath).ID;
            }
        }

    }

    partial class GraphGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        [XmlIgnore()]
        public virtual string Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
                OnPropertyChanged("Description");
            }
        }

        public T[] GetNodes<T>()
        {
            List<T> nodes = new List<T>();
            foreach (object node in Item)
            {
                if (node is T)
                {
                    nodes.Add((T)node);
                }
            }
            return nodes.ToArray();
        }

        public JumpNode[] JumpNodes
        {
            get
            {
                return GetNodes<JumpNode>();
            }
        }

        public GetKeyNode[] GetKeyNodes
        {
            get
            {
                return GetNodes<GetKeyNode>();
            }
        }

        public void ClearGetKeyNodes()
        {
            foreach (GetKeyNode node in GetKeyNodes)
            {
                this.Item.Remove(node);
            }
        }

        public InvokeNode[] InvokeNodes
        {
            get
            {
                return GetNodes<InvokeNode>();
            }
        }



        public RecordNode[] RecordNodes
        {
            get
            {
                return GetNodes<RecordNode>();
            }
        }

        public PlayNode[] PlayNodes
        {
            get
            {
                return GetNodes<PlayNode>();
            }
        }


        public NodeGroup[] NodeGroups
        {
            get
            {
                return GetNodes<NodeGroup>();
            }
        }

        public DivertNode[] DivertNodes
        {
            get
            {
                return GetNodes<DivertNode>();
            }
        }

        public FaxNode[] FaxNodes
        {
            get
            {
                return GetNodes<FaxNode>();
            }
        }

        public SelectNode[] SelectNodes
        {
            get
            {
                return GetNodes<SelectNode>();
            }
        }

        /// <summary>
        /// Gets the root node for which the path is specified.
        /// </summary>
        /// <param name="nodePath">eg: PlayNodes[0]</param>
        protected Node GetNodeByPath(string nodePath)
        {
            string nodeName;
            int index;
            var groups = Regex.Match(nodePath, @"(?<name>.+)\[(?<index>\d+)\]").Groups;
            nodeName = groups["name"].Value;
            index = int.Parse(groups["index"].Value);
            return this.GetPropertyValue<Node[]>(nodeName)[index];
        }

        public Node FindNodeById(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
                return null;
            Node node = FindNodeById(nodeId, this);
            Logger.WriteIf(node == null, LogType.Exception, "Invalid Node ID: {0} in graph: {1}", nodeId, this.Description);
            return node;
        }

        private Node FindNodeById(string nodeId, GraphGroup group)
        {
            try
            {
                foreach (object item in group.Item)
                {
                    if (item is Node)
                    {
                        if ((item as Node).ID == nodeId)
                            return item as Node;
                    }
                    else
                    {
                        Node foundedNode = FindNodeById(nodeId, item as GraphGroup);
                        if ((foundedNode != null))
                            return foundedNode;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(LogType.Exception, ex);
            }
            return null;
        }

        /// <summary>
        /// This is equivalent to Item, except that its name makes more sanse!
        /// </summary>
        [XmlIgnore()]
        public ArrayList Items
        {
            get
            {
                return Item;
            }
            set
            {
                Item = value;
            }
        }


    }

    partial class Graph
    {
        [XmlIgnore]
        private int MaxID;

        private int GetMaxNodeID(ArrayList items)
        {
            foreach (object item in items)
            {
                if (item is NodeGroup)
                {
                    GetMaxNodeID((item as NodeGroup).Item);
                }
                else
                {
                    string id = ((Node)item).ID;
                    if (!string.IsNullOrEmpty(id) && id.StartsWith("#"))
                    {
                        int idValue = int.Parse(id.Substring(1));
                        MaxID = Math.Max(MaxID, idValue);
                    }
                }
            }

            return MaxID;
        }

        public XElement ToXElement()
        {
            return XElement.Load(new StringReader(this.Serialize(true)));
        }

        public void CheckMaxID()
        {
            MaxID = 0;
            GetMaxNodeID(this.Item);
        }

        public string NewID()
        {
            MaxID++;
            return "#" + MaxID;
        }
    }

    partial class GetKeyNode
    {
        public void DeleteResultByValue(string value)
        {
            foreach (NodeResult result in this.NodeResult)
            {
                if (result.Value == value)
                {
                    this.NodeResult.Remove(result);
                    return;
                }
            }
        }

        public string FindTargetByResultValue(string value)
        {
            foreach (NodeResult result in this.NodeResult)
            {
                //TODO : contains is not always correct, because sometimes for more than 1 digits 
                if (!string.IsNullOrEmpty(result.Value) && result.Value.Contains(value))
                {
                    return result.TargetNode;
                }
            }
            return null;
        }

        public string FindDefaultTarget()
        {
            foreach (NodeResult result in this.NodeResult)
            {
                if (result.Value == null)
                    return result.TargetNode;
            }
            return null;
        }

        public bool HasDefaultTarget()
        {
            foreach (NodeResult result in this.NodeResult)
            {
                if (result.Value == null)
                    return true;
            }
            return false;
        }
    }

    partial class InvokeNode
    {
        [XmlIgnore]
        public int InvokeTimes = 0;

        [XmlIgnore]
        public DateTime? LastInvokeTime;

        public InvokeNode()
        {
        }

        //public Dictionary<string, InvokeNodeArg> Args
        //{
        //    get
        //    {
        //        return Arg.ToDictionary(arg => arg.Name);
        //    }
        //}

        public InvokeNode(string id, string functionName, string argName, string argValue, string defaultTargetNode)
        {
            this.ID = id;
            if (!string.IsNullOrEmpty(functionName))
                this.Function = functionName;

            if (!string.IsNullOrEmpty(argName) | !string.IsNullOrEmpty(argValue))
            {
                this.Arg.Add(new InvokeNodeArg
                {
                    Name = argName,
                    Value = argValue
                });
            }

            if (!string.IsNullOrEmpty(defaultTargetNode))
            {
                this.NodeResult.Add(new NodeResult
                {
                    TargetNode = defaultTargetNode
                });
            }
        }

        public string GetArgsAsString()
        {
            List<string> args = new List<string>();
            if (Arg.Count > 0)
            {
                foreach (InvokeNodeArg arg in this.Arg)
                {
                    args.Add(string.Format("{0}={1}", arg.Name, arg.Value));
                }
            }
            return string.Join(",", args.ToArray());
        }

        public void SetStringArgs(string args)
        {
            string[] argsList = args.Split('&', ',');
            this.Arg.Clear();
            foreach (string argitem in argsList)
            {
                if (argitem.IndexOf("=") > 0)
                {
                    this.Arg.Add(new InvokeNodeArg
                    {
                        _Name = argitem.Split('=')[0],
                        _Value = argitem.Split('=')[1]
                    });
                }
            }
        }

        public InvokeNodeArg GetArgByName(string argName)
        {
            foreach (InvokeNodeArg arg in this.Arg)
            {
                if (string.Compare(arg.Name.Trim(), argName, true) == 0)
                    return arg;
            }
            return null;
        }

        public string GetResultTarget(object value)
        {
            if (this.NodeResult == null)
                return null;

            string valueString = null;
            if (value == null)
            {
                valueString = string.Empty;
            }
            else
            {
                valueString = value.ToString();
            }

            foreach (NodeResult result in this.NodeResult)
            {
                if (string.Compare(result.Value, valueString, true) == 0)
                    return result.TargetNode;
            }

            foreach (NodeResult result in this.NodeResult)
            {
                if (string.IsNullOrEmpty(result.Value))
                    return result.TargetNode;
            }

            Logger.WriteIf(!string.IsNullOrEmpty((string)valueString), LogType.Info, "result '{0}' as invoking method '{1}' not found!", valueString, this.Function);
            return null;
        }

        public NodeResult FindByTarget(string targetID)
        {
            foreach (NodeResult result in this.NodeResult)
            {
                if (result.TargetNode == targetID)
                    return result;
            }
            return null;
        }

        public void DeleteResultByValue(string value)
        {
            foreach (NodeResult result in this.NodeResult)
            {
                if (result.Value == value)
                {
                    this.NodeResult.Remove(result);
                    return;
                }
            }
        }
    }

    partial class FaxNode
    {
        public FaxNode()
        {
        }

        public FaxNode(string id, string templateName, int templateId, bool readFromFile, string argName, string argValue, string defaultTargetNode)
        {
            this.ID = id;

            this.ReadFromFile = readFromFile;
            
            if (!string.IsNullOrEmpty(templateName))
                this.TemplateName = templateName;

            if (!string.IsNullOrEmpty(argName) | !string.IsNullOrEmpty(argValue))
            {
                this.Arg.Add(new FaxNodeArg
                {
                    Name = argName,
                    Value = argValue
                });
            }

            if (!string.IsNullOrEmpty(defaultTargetNode))
            {
                this.NodeResult.Add(new NodeResult
                {
                    TargetNode = defaultTargetNode
                });
            }
        }

        public string GetArgsAsString()
        {
            List<string> args = new List<string>();
            if (Arg.Count > 0)
            {
                foreach (FaxNodeArg arg in this.Arg)
                {
                    args.Add(string.Format("{0}={1}", arg.Name, arg.Value));
                }
            }
            return string.Join(",", args.ToArray());
        }

        public void SetStringArgs(string args)
        {
            string[] argsList = args.Split('&', ',');
            this.Arg.Clear();
            foreach (string argitem in argsList)
            {
                if (argitem.IndexOf("=") > 0)
                {
                    this.Arg.Add(new FaxNodeArg
                    {
                        _Name = argitem.Split('=')[0],
                        _Value = argitem.Split('=')[1]
                    });
                }
            }
        }

        public FaxNodeArg GetArgByName(string argName)
        {
            foreach (FaxNodeArg arg in this.Arg)
            {
                if (string.Compare(arg.Name.Trim(), argName, true) == 0)
                    return arg;
            }
            return null;
        }
    }

    partial class SelectNode
    {
        public string GetCallerIDMatchTarget(string callerID)
        {
            foreach (SelectNodeMatchCallerID match in MatchCallerIDs)
            {
                if (!string.IsNullOrEmpty(match.Code))
                {
                    string[] splittedCodes = match.Code.Split(' ', ',', ';', 'و');
                    foreach (var code in splittedCodes)
                    {
                        if (Regex.IsMatch(code, @"\d+-\d+"))
                        {
                            string startString = code.Split('-').First();
                            string endString = code.Split('-').Last();

                            int cuttingIndex = startString.Length <= endString.Length ? startString.Length : endString.Length;

                            long start = long.Parse(startString.Substring(0, cuttingIndex));
                            long end = long.Parse(endString.Substring(0, cuttingIndex));
                            long callerId = long.Parse(callerID.Substring(0, cuttingIndex));

                            if (end >= start)
                            {
                                if (callerId >= start && callerId <= end)
                                    return match.TargetNode;
                            }
                            else
                                if (callerId >= end && callerId <= start)
                                    return match.TargetNode;
                        }
                        else
                        {
                            if (code != "" && callerID.StartsWith(code))
                                return match.TargetNode;
                        }
                    }
                }
            }

            foreach (SelectNodeMatchCallerID match in MatchCallerIDs)
            {
                if (string.IsNullOrEmpty(match.Code))
                {
                    return match.TargetNode;
                }
            }

            return null;
        }
        public string GetTimeMatchTarget()
        {
            foreach (SelectNodeMatchTime match in MatchTimes)
            {
                if (match.StartTime.HasValue && match.StartTime.Value.TimeOfDay <= DateTime.Now.TimeOfDay && match.EndTime.HasValue && match.EndTime.Value.TimeOfDay >= DateTime.Now.TimeOfDay)
                    return match.TargetNode;
            }

            foreach (SelectNodeMatchTime match in MatchTimes)
            {
                if (!match.StartTime.HasValue && !match.EndTime.HasValue)
                    return match.TargetNode;
            }

            return null;
        }

        public string GetDateMatchTarget()
        {
            foreach (SelectNodeMatchDate match in MatchDates)
            {
                if (match.StartDate.HasValue && match.StartDate.Value <= DateTime.Now.Date && match.EndDate.HasValue && match.EndDate.Value >= DateTime.Now.Date)
                    return match.TargetNode;
            }

            foreach (SelectNodeMatchDate match in MatchDates)
            {
                if (!match.StartDate.HasValue && !match.StartDate.HasValue)
                    return match.TargetNode;
            }

            return null;
        }
        public SelectNodeMatchCallerID Find(string targetNodeID)
        {
            foreach (SelectNodeMatchCallerID match in MatchCallerIDs)
            {
                if (match.TargetNode == targetNodeID)
                {
                    return match;
                }
            }
            return null;
        }

        public SelectNodeMatchTime FindTimeMatch(string targetNodeID)
        {
            foreach (SelectNodeMatchTime match in MatchTimes)
            {
                if (match.TargetNode == targetNodeID)
                {
                    return match;
                }
            }
            return null;
        }

    }

    partial class DivertNode
    {
        [XmlAttribute(AttributeName = "DynamicTargetPhone")]
        public string _DynamicTargetPhone;

        [XmlIgnore()]
        public string DynamicTargetPhone
        {
            get
            {
                return this._DynamicTargetPhone;
            }
            set
            {
                this._DynamicTargetPhone = value;
                OnPropertyChanged("DynamicTargetPhone");
            }
        }
    }
}
