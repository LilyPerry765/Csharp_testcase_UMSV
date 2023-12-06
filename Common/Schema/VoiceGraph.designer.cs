using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System;

namespace UMSV.Schema
{
    public partial class VoiceGraphUtility
    {

        public const string SchemaNamespace = "http://tempuri.org/VoiceGraph.xsd";

        public static string Serialize<T>(T o, bool indented)
        {
            StringWriter writer = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            if (indented)
            {
                xmlWriter.Formatting = Formatting.Indented;
            }
            else
            {
                xmlWriter.Formatting = Formatting.None;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(xmlWriter, o);
            return writer.ToString();
        }

        public static T Deserialize<T>(string objectXml)
        {
            StringReader reader = new StringReader(objectXml);
            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T classObject = default(T);
            if (!String.IsNullOrEmpty(objectXml))
                classObject = ((T)(serializer.Deserialize(reader)));

            reader.Close();
            return classObject;
        }
    }

    [Serializable(), XmlType(TypeName = "Node", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class Node
    {

        [XmlAttribute(AttributeName = "ID")]
        public string _ID;

        [XmlAttribute(AttributeName = "Description")]
        public string _Description;

        [XmlAttribute(AttributeName = "ClearDigits")]
        public bool _ClearDigits = true;

        [XmlAttribute(AttributeName = "Tag")]
        public string _Tag;

        [XmlIgnore()]
        public virtual string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
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
            }
        }

        [XmlIgnore()]
        public virtual bool ClearDigits
        {
            get
            {
                return this._ClearDigits;
            }
            set
            {
                this._ClearDigits = value;
            }
        }

        [XmlIgnore()]
        public virtual string Tag
        {
            get
            {
                return this._Tag;
            }
            set
            {
                this._Tag = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "Graph", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class Graph : GraphGroup
    {
        public virtual string Serialize()
        {
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(Graph));
            serializer.Serialize(writer, this);
            return writer.ToString();
        }

        public virtual string Serialize(bool indented)
        {
            StringWriter writer = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            if (indented)
            {
                xmlWriter.Formatting = Formatting.Indented;
            }
            else
            {
                xmlWriter.Formatting = Formatting.None;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(Graph));
            serializer.Serialize(xmlWriter, this);
            return writer.ToString();
        }

        public static Graph Deserialize(string objectXml)
        {
            Graph functionReturnValue = default(Graph);
            StringReader reader = new StringReader(objectXml);
            XmlSerializer serializer = new XmlSerializer(typeof(Graph));
            functionReturnValue = serializer.Deserialize(reader) as Graph;
            reader.Close();
            return functionReturnValue;
        }
    }

    [Serializable(), XmlRoot(ElementName = "RecordNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class RecordNode : WithTimerNode
    {

        [XmlAttribute(AttributeName = "StopKey")]
        public string _StopKey;

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "CancelOnDisconnect")]
        public bool _CancelOnDisconnect = false;

        [XmlIgnore()]
        public virtual string StopKey
        {
            get
            {
                return this._StopKey;
            }
            set
            {
                this._StopKey = value;
            }
        }

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
            }
        }

        [XmlIgnore()]
        public virtual bool CancelOnDisconnect
        {
            get
            {
                return this._CancelOnDisconnect;
            }
            set
            {
                this._CancelOnDisconnect = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "InvokeNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class InvokeNode : WithTimerNode
    {

        [XmlElement(ElementName = "Arg", Type = typeof(InvokeNodeArg), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<InvokeNodeArg> _Arg;

        [XmlAttribute(AttributeName = "Function")]
        public string _Function;

        [XmlIgnore()]
        public virtual List<InvokeNodeArg> Arg
        {
            get
            {
                if ((_Arg == null))
                {
                    _Arg = new List<InvokeNodeArg>();
                }
                return this._Arg;
            }
            set
            {
                this._Arg = value;
            }
        }

        [XmlElement(ElementName = "NodeResult", Type = typeof(NodeResult), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<NodeResult> _NodeResult;

        [XmlIgnore()]
        public virtual List<NodeResult> NodeResult
        {
            get
            {
                if ((_NodeResult == null))
                {
                    _NodeResult = new List<NodeResult>();
                }
                return this._NodeResult;
            }
            set
            {
                this._NodeResult = value;
            }
        }

        [XmlIgnore()]
        public virtual string Function
        {
            get
            {
                return this._Function;
            }
            set
            {
                this._Function = value;
            }
        }
    }

    [Serializable(), XmlType(TypeName = "InvokeNodeArg", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class InvokeNodeArg
    {

        [XmlAttribute(AttributeName = "Name")]
        public string _Name;

        [XmlAttribute(AttributeName = "Value")]
        public string _Value;

        [XmlIgnore()]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        [XmlIgnore()]
        public virtual string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "FaxNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class FaxNode : WithTimerNode
    {

        [XmlElement(ElementName = "Arg", Type = typeof(FaxNodeArg), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<FaxNodeArg> _Arg;

        [XmlIgnore()]
        public virtual List<FaxNodeArg> Arg
        {
            get
            {
                if ((_Arg == null))
                {
                    _Arg = new List<FaxNodeArg>();
                }
                return this._Arg;
            }
            set
            {
                this._Arg = value;
            }
        }

        [XmlAttribute(AttributeName = "TemplateName")]
        public string _TemplateName;

        [XmlAttribute(AttributeName = "TemplateID")]
        public int _TemplateID;

        [XmlAttribute(AttributeName = "ReadFromFile")]
        public bool _ReadFromFile = false;

        [XmlElement(ElementName = "NodeResult", Type = typeof(NodeResult), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<NodeResult> _NodeResult;

        [XmlIgnore()]
        public virtual List<NodeResult> NodeResult
        {
            get
            {
                if ((_NodeResult == null))
                {
                    _NodeResult = new List<NodeResult>();
                }
                return this._NodeResult;
            }
            set
            {
                this._NodeResult = value;
            }
        }

        [XmlIgnore()]
        public virtual string TemplateName
        {
            get
            {
                return this._TemplateName;
            }
            set
            {
                this._TemplateName = value;
                OnPropertyChanged("TemplateName");
            }
        }

        [XmlIgnore()]
        public virtual int TemplateID
        {
            get
            {
                return this._TemplateID;
            }
            set
            {
                this._TemplateID = value;
                OnPropertyChanged("TemplateID");
            }
        }

        [XmlIgnore()]
        public virtual bool ReadFromFile
        {
            get
            {
                return this._ReadFromFile;
            }
            set
            {
                this._ReadFromFile = value;
                OnPropertyChanged("ReadFromFile");
            }
        }
    }

    [Serializable(), XmlType(TypeName = "FaxNodeArg", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class FaxNodeArg
    {

        [XmlAttribute(AttributeName = "Name")]
        public string _Name;

        [XmlAttribute(AttributeName = "Value")]
        public string _Value;

        [XmlIgnore()]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }

        [XmlIgnore()]
        public virtual string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "GetKeyNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class GetKeyNode : WithTimerNode
    {

        [XmlElement(ElementName = "NodeResult", Type = typeof(NodeResult), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<NodeResult> _NodeResult;

        [XmlIgnore()]
        public virtual List<NodeResult> NodeResult
        {
            get
            {
                if ((_NodeResult == null))
                {
                    _NodeResult = new List<NodeResult>();
                }
                return this._NodeResult;
            }
            set
            {
                this._NodeResult = value;
            }
        }

        [XmlAttribute(AttributeName = "MaxDigits")]
        public int _MaxDigits;

        [XmlIgnore()]
        public bool _MaxDigitsSpecified;

        [XmlAttribute(AttributeName = "MaxDigitsNode")]
        public string _MaxDigitsNode;

        [XmlAttribute(AttributeName = "EndKey")]
        public string _EndKey;

        [XmlAttribute(AttributeName = "MinDigits")]
        public int _MinDigits = 0;

        [XmlIgnore()]
        public virtual int? MaxDigits
        {
            get
            {
                if (_MaxDigitsSpecified)
                {
                    return this._MaxDigits;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._MaxDigitsSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._MaxDigits = value.Value;
                }
            }
        }

        [XmlIgnore()]
        public virtual string MaxDigitsNode
        {
            get
            {
                return this._MaxDigitsNode;
            }
            set
            {
                this._MaxDigitsNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string EndKey
        {
            get
            {
                return this._EndKey;
            }
            set
            {
                this._EndKey = value;
            }
        }

        [XmlIgnore()]
        public virtual int MinDigits
        {
            get
            {
                return this._MinDigits;
            }
            set
            {
                this._MinDigits = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "PlayNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class PlayNode : Node
    {

        [XmlElement(ElementName = "Voice", Type = typeof(Voice), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<Voice> _Voice;

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "BoxNo")]
        public string _BoxNo;

        [XmlAttribute(AttributeName = "IgnoreKeyPress")]
        public bool _IgnoreKeyPress = false;


        [XmlIgnore()]
        public virtual List<Voice> Voice
        {
            get
            {
                if ((_Voice == null))
                {
                    _Voice = new List<Voice>();
                }
                return this._Voice;
            }
            set
            {
                this._Voice = value;
            }
        }

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
                OnPropertyChanged("TargetNode");
            }
        }

        [XmlIgnore()]
        public virtual string BoxNo
        {
            get
            {
                return this._BoxNo;
            }
            set
            {
                this._BoxNo = value;
            }
        }

        [XmlIgnore()]
        public virtual bool IgnoreKeyPress
        {
            get
            {
                return this._IgnoreKeyPress;
            }
            set
            {
                this._IgnoreKeyPress = value;
            }
        }

    }


    [Serializable(), XmlRoot(ElementName = "Voice", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class Voice
    {

        [XmlAttribute(AttributeName = "ID")]
        public string _ID;

        [XmlAttribute(AttributeName = "Name")]
        public string _Name;

        [XmlAttribute(AttributeName = "Description")]
        public string _Description;

        [XmlAttribute(AttributeName = "Type")]
        public byte _Type = 0;

        [XmlAttribute(AttributeName = "Group")]
        public byte _Group = 0;

        [XmlIgnore()]
        public bool _GroupSpecified;

        [XmlIgnore()]
        public virtual string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                this._ID = value;
            }
        }

        [XmlIgnore()]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
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
            }
        }

        [XmlIgnore()]
        public virtual byte Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        [XmlIgnore()]
        public virtual byte? Group
        {
            get
            {
                if (_GroupSpecified)
                {
                    return this._Group;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._GroupSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._Group = value.Value;
                }
            }
        }
    }

    public enum NodeType
    {
        Play,
        GetKey,
        Invoke,
        Record,
        Jump,
        Divert,
        Select,
        Fax
    }

    [Serializable(), XmlRoot(ElementName = "NodeGroup", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class NodeGroup : GraphGroup
    {
        [XmlAttribute(AttributeName = "LastNode")]
        public string _LastNode;

        [XmlIgnore()]
        public virtual string LastNode
        {
            get
            {
                return this._LastNode;
            }
            set
            {
                this._LastNode = value;
            }
        }
    }

    [Serializable(), XmlType(TypeName = "GraphGroup", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class GraphGroup
    {
        [XmlAttribute(AttributeName = "StartNode")]
        public string _StartNode;

        [XmlIgnore()]
        public virtual string StartNode
        {
            get
            {
                return this._StartNode;
            }
            set
            {
                this._StartNode = value;
            }
        }

        [XmlElement(ElementName = "RecordNode", Type = typeof(RecordNode)), XmlElement(ElementName = "InvokeNode", Type = typeof(InvokeNode)), XmlElement(ElementName = "GetKeyNode", Type = typeof(GetKeyNode)), XmlElement(ElementName = "PlayNode", Type = typeof(PlayNode)), XmlElement(ElementName = "NodeGroup", Type = typeof(NodeGroup)), XmlElement(ElementName = "JumpNode", Type = typeof(JumpNode)), XmlElement(ElementName = "DivertNode", Type = typeof(DivertNode)), XmlElement(ElementName = "FaxNode", Type = typeof(FaxNode)), XmlElement(ElementName = "SelectNode", Type = typeof(SelectNode))]
        public ArrayList _Item;

        [XmlAttribute(AttributeName = "Description")]
        public string _Description;

        [XmlAttribute(AttributeName = "Tag")]
        public string _Tag;

        [XmlIgnore()]
        public virtual ArrayList Item
        {
            get
            {
                if ((_Item == null))
                {
                    _Item = new ArrayList();
                }
                return this._Item;
            }
            set
            {
                this._Item = value;
            }
        }

        [XmlIgnore()]
        public virtual string Tag
        {
            get
            {
                return this._Tag;
            }
            set
            {
                this._Tag = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "NodeResult", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class NodeResult
    {

        [XmlAttribute(AttributeName = "Value")]
        public string _Value;

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "Description")]
        public string _Description;

        [XmlIgnore()]
        public virtual string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
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
            }
        }
    }

    [Serializable(), XmlType(TypeName = "WithTimerNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class WithTimerNode : Node
    {

        [XmlAttribute(AttributeName = "Timeout")]
        public int _Timeout;

        [XmlIgnore()]
        public bool _TimeoutSpecified;

        [XmlAttribute(AttributeName = "TimeoutNode")]
        public string _TimeoutNode;

        [XmlIgnore()]
        public virtual int? Timeout
        {
            get
            {
                if (_TimeoutSpecified)
                {
                    return this._Timeout;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._TimeoutSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._Timeout = value.Value;
                }
            }
        }

        [XmlIgnore()]
        public virtual string TimeoutNode
        {
            get
            {
                return this._TimeoutNode;
            }
            set
            {
                this._TimeoutNode = value;
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "JumpNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class JumpNode : Node
    {

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "TargetGraph")]
        public string _TargetGraph;

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
                OnPropertyChanged("TargetNode");
            }
        }

        [XmlIgnore()]
        public virtual string TargetGraph
        {
            get
            {
                return this._TargetGraph;
            }
            set
            {
                this._TargetGraph = value;
                OnPropertyChanged("TargetGraph");
            }
        }
    }

    [Serializable(), XmlRoot(ElementName = "DivertNode", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class DivertNode : WithTimerNode
    {
        [XmlAttribute(AttributeName = "MaxTalkTime")]
        public int _MaxTalkTime = 600;

        [XmlAttribute(AttributeName = "CallerPostfix")]
        public string _CallerPostfix;

        [XmlAttribute(AttributeName = "CallerPrefix")]
        public string _CallerPrefix;

        [XmlAttribute(AttributeName = "CalleePrefix")]
        public string _CalleePrefix;

        [XmlAttribute(AttributeName = "CalleePostfix")]
        public string _CalleePostfix;

        [XmlAttribute(AttributeName = "CallerDeleteFromStart")]
        public int _CallerDeleteFromStart;

        [XmlAttribute(AttributeName = "CalleeDeleteFromStart")]
        public int _CalleeDeleteFromStart;


        [XmlAttribute(AttributeName = "CallerDeleteFromEnd")]
        public int _CallerDeleteFromEnd;

        [XmlAttribute(AttributeName = "CalleeDeleteFromEnd")]
        public int _CalleeDeleteFromEnd;


        [XmlAttribute(AttributeName = "ClearAllSource")]
        public bool _ClearAllSource;

        [XmlAttribute(AttributeName = "TargetPhone")]
        public string _TargetPhone;

        [XmlAttribute(AttributeName = "TargetTeam")]
        public string _TargetTeam;

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "BusyNode")]
        public string _BusyNode;

        [XmlAttribute(AttributeName = "UnallocatedNumberNode")]
        public string _UnallocatedNumberNode;

        [XmlAttribute(AttributeName = "SubscriberAbsentNode")]
        public string _SubscriberAbsentNode;

        [XmlAttribute(AttributeName = "FullQueueNode")]
        public string _FullQueueNode;

        [XmlAttribute(AttributeName = "QueueSize")]
        public int _QueueSize = 0;

        [XmlAttribute(AttributeName = "QueueSizePerOnlineUsers")]
        public decimal _QueueSizePerOnlineUsers = 0;

        [XmlAttribute(AttributeName = "WaitMessage")]
        public string _WaitMessage;

        [XmlAttribute(AttributeName = "WaitSound")]
        public string _WaitSound;

        [XmlAttribute(AttributeName = "RecordVoice")]
        public bool _RecordVoice = false;

        [XmlAttribute(AttributeName = "ProxyMode")]
        public bool _ProxyMode = true;

        [XmlAttribute(AttributeName = "FailureNode")]
        public string _FailureNode;

        [XmlAttribute(AttributeName = "MaxTalkTimeNode")]
        public string _MaxTalkTimeNode;

        [XmlAttribute(AttributeName = "ForwardAnswer")]
        public bool _ForwardAnswer;

        [XmlElement(ElementName = "NodeResult", Type = typeof(NodeResult), Namespace = "http://tempuri.org/VoiceGraph.xsd")]
        public List<NodeResult> _NodeResult;

        [XmlIgnore()]
        public virtual List<NodeResult> NodeResult
        {
            get
            {
                if ((_NodeResult == null))
                {
                    _NodeResult = new List<NodeResult>();
                }
                return this._NodeResult;
            }
            set
            {
                this._NodeResult = value;
            }
        }

        [XmlIgnore()]
        public virtual string TargetPhone
        {
            get
            {
                return this._TargetPhone;
            }
            set
            {
                this._TargetPhone = value;
                OnPropertyChanged("TargetPhone");
            }
        }

        [XmlIgnore()]
        public virtual string TargetTeam
        {
            get
            {
                return this._TargetTeam;
            }
            set
            {
                this._TargetTeam = value;
                OnPropertyChanged("TargetTeam");
            }
        }

        [XmlIgnore()]
        public virtual string BusyNode
        {
            get
            {
                return this._BusyNode;
            }
            set
            {
                this._BusyNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string SubscriberAbsentNode
        {
            get
            {
                return this._SubscriberAbsentNode;
            }
            set
            {
                this._SubscriberAbsentNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string UnallocatedNumberNode
        {
            get
            {
                return this._UnallocatedNumberNode;
            }
            set
            {
                this._UnallocatedNumberNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string FullQueueNode
        {
            get
            {
                return this._FullQueueNode;
            }
            set
            {
                this._FullQueueNode = value;
            }
        }


        [XmlIgnore()]
        public virtual int QueueSize
        {
            get
            {
                return this._QueueSize;
            }
            set
            {
                this._QueueSize = value;
            }
        }

        [XmlIgnore()]
        public virtual decimal QueueSizePerOnlineUsers
        {
            get
            {
                return this._QueueSizePerOnlineUsers;
            }
            set
            {
                this._QueueSizePerOnlineUsers = value;
            }
        }

        [XmlIgnore()]
        public virtual int MaxTalkTime
        {
            get
            {
                return this._MaxTalkTime;
            }
            set
            {
                this._MaxTalkTime = value;
            }
        }

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string WaitMessage
        {
            get
            {
                return this._WaitMessage;
            }
            set
            {
                this._WaitMessage = value;
            }
        }

        [XmlIgnore()]
        public virtual string WaitSound
        {
            get
            {
                return this._WaitSound;
            }
            set
            {
                this._WaitSound = value;
            }
        }

        [XmlIgnore()]
        public virtual bool RecordVoice
        {
            get
            {
                return this._RecordVoice;
            }
            set
            {
                this._RecordVoice = value;
            }
        }

        [XmlIgnore()]
        public virtual bool ProxyMode
        {
            get
            {
                return this._ProxyMode;
            }
            set
            {
                this._ProxyMode = value;
            }
        }

        [XmlIgnore()]
        public virtual string FailureNode
        {
            get
            {
                return this._FailureNode;
            }
            set
            {
                this._FailureNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string MaxTalkTimeNode
        {
            get
            {
                return this._MaxTalkTimeNode;
            }
            set
            {
                this._MaxTalkTimeNode = value;
            }
        }

        [XmlIgnore()]
        public virtual bool ForwardAnswer
        {
            get
            {
                return this._ForwardAnswer;
            }
            set
            {
                this._ForwardAnswer = value;
            }
        }

        [XmlIgnore()]
        public string CallerPrefix
        {
            get
            {
                return _CallerPrefix;
            }
            set
            {
                _CallerPrefix = value;
            }
        }
        [XmlIgnore()]
        public string CallerPostfix
        {
            get
            {
                return _CallerPostfix;
            }
            set
            {
                _CallerPostfix = value;
            }
        }
        [XmlIgnore()]
        public int CallerDeleteFromStart
        {
            get
            {
                return _CallerDeleteFromStart;
            }
            set
            {
                _CallerDeleteFromStart = value;
            }
        }
        [XmlIgnore()]
        public int CallerDeleteFromEnd
        {
            get
            {
                return _CallerDeleteFromEnd;
            }
            set
            {
                _CallerDeleteFromEnd = value;
            }
        }

        [XmlIgnore()]
        public string CalleePrefix
        {
            get
            {
                return _CalleePrefix;
            }
            set
            {
                _CalleePrefix = value;
            }
        }
        [XmlIgnore()]
        public string CalleePostfix
        {
            get
            {
                return _CalleePostfix;
            }
            set
            {
                _CalleePostfix = value;
            }
        }
        [XmlIgnore()]
        public int CalleeDeleteFromStart
        {
            get
            {
                return _CalleeDeleteFromStart;
            }
            set
            {
                _CalleeDeleteFromStart = value;
            }
        }
        [XmlIgnore()]
        public int CalleeDeleteFromEnd
        {
            get
            {
                return _CalleeDeleteFromEnd;
            }
            set
            {
                _CalleeDeleteFromEnd = value;
            }
        }

        [XmlIgnore()]
        public bool ClearAllSource
        {
            get
            {
                return _ClearAllSource;
            }
            set
            {
                _ClearAllSource = value;
            }
        }


    }

    [Serializable()]
    [XmlRoot(ElementName = "SelectNode", Namespace = VoiceGraphUtility.SchemaNamespace, IsNullable = false)]
    public partial class SelectNode : Node, INotifyPropertyChanged
    {

        [XmlElement(ElementName = "MatchCallerID")]
        public List<SelectNodeMatchCallerID> _MatchCallerIDs;

        public const string MatchCallerIDsProperty = "MatchCallerIDs";

        [XmlElement(ElementName = "MatchTime")]
        public List<SelectNodeMatchTime> _MatchTimes;

        public const string MatchTimesProperty = "MatchTimes";

        [XmlElement(ElementName = "MatchDate")]
        public List<SelectNodeMatchDate> _MatchDates;

        public const string MatchDatesProperty = "MatchDates";

        [XmlAttribute(AttributeName = "DefaultNode")]
        public string _DefaultNode;

        public const string DefaultNodeProperty = "DefaultNode";

        [XmlIgnore()]
        public List<SelectNodeMatchCallerID> MatchCallerIDs
        {
            get
            {
                if ((_MatchCallerIDs == null))
                {
                    _MatchCallerIDs = new List<SelectNodeMatchCallerID>();
                }
                return _MatchCallerIDs;
            }
            set
            {
                if ((_MatchCallerIDs != value))
                {
                    this._MatchCallerIDs = value;
                    this.SendPropertyChanged("MatchCallerIDs");
                }
            }
        }

        [XmlIgnore()]
        public List<SelectNodeMatchTime> MatchTimes
        {
            get
            {
                if ((_MatchTimes == null))
                {
                    _MatchTimes = new List<SelectNodeMatchTime>();
                }
                return _MatchTimes;
            }
            set
            {
                if ((_MatchTimes != value))
                {
                    this._MatchTimes = value;
                    this.SendPropertyChanged("MatchTimes");
                }
            }
        }

        [XmlIgnore()]
        public List<SelectNodeMatchDate> MatchDates
        {
            get
            {
                if ((_MatchDates == null))
                {
                    _MatchDates = new List<SelectNodeMatchDate>();
                }
                return _MatchDates;
            }
            set
            {
                if ((_MatchDates != value))
                {
                    this._MatchDates = value;
                    this.SendPropertyChanged("MatchDates");
                }
            }
        }

        [XmlIgnore()]
        public string DefaultNode
        {
            get
            {
                return this._DefaultNode;
            }
            set
            {
                if ((_DefaultNode != value))
                {
                    this._DefaultNode = value;
                    this.SendPropertyChanged("DefaultNode");
                }
            }
        }

        private event PropertyChangedEventHandler PropertyChanged;

        private void SendPropertyChanged(string propertyName)
        {
            if ((this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [Serializable(), XmlType(TypeName = "SelectNodeMatchTime", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class SelectNodeMatchTime
    {

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "Comment")]
        public string _Comment;

        [XmlAttribute(AttributeName = "StartTime")]
        public System.DateTime _StartTime;

        [XmlIgnore()]
        public bool _StartTimeSpecified;

        [XmlIgnore()]
        public virtual System.DateTime? StartTime
        {
            get
            {
                if (_StartTimeSpecified)
                {
                    return this._StartTime;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._StartTimeSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._StartTime = value.Value;
                }
            }
        }

        [XmlAttribute(AttributeName = "EndTime")]
        public System.DateTime _EndTime;

        [XmlIgnore()]
        public bool _EndTimeSpecified;

        [XmlIgnore()]
        public virtual System.DateTime? EndTime
        {
            get
            {
                if (_EndTimeSpecified)
                {
                    return this._EndTime;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._EndTimeSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._EndTime = value.Value;
                }
            }
        }

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                this._Comment = value;
            }
        }


        [XmlAttribute(AttributeName = "StartDate")]
        public System.DateTime _StartDate;

        [XmlIgnore()]
        public bool _StartDateSpecified;

        [XmlIgnore()]
        public virtual System.DateTime? StartDate
        {
            get
            {
                if (_StartDateSpecified)
                {
                    return this._StartDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._StartDateSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._StartDate = value.Value;
                }
            }
        }

        [XmlAttribute(AttributeName = "EndDate")]
        public System.DateTime _EndDate;

        [XmlIgnore()]
        public bool _EndDateSpecified;

        [XmlIgnore()]
        public virtual System.DateTime? EndDate
        {
            get
            {
                if (_EndDateSpecified)
                {
                    return this._EndDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this._EndDateSpecified = value.HasValue;
                if (value.HasValue)
                {
                    this._EndDate = value.Value;
                }
            }
        }
    }

    [Serializable(), XmlType(TypeName = "SelectNodeMatchCallerID", Namespace = "http://tempuri.org/VoiceGraph.xsd")]
    public partial class SelectNodeMatchCallerID
    {

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        [XmlAttribute(AttributeName = "Code")]
        public string _Code;

        [XmlAttribute(AttributeName = "Comment")]
        public string _Comment;

        [XmlIgnore()]
        public virtual string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                this._TargetNode = value;
            }
        }

        [XmlIgnore()]
        public virtual string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this._Code = value;
            }
        }

        [XmlIgnore()]
        public virtual string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                this._Comment = value;
            }
        }
    }

    [Serializable()]
    [XmlType(TypeName = "SelectNodeMatchDate", Namespace = VoiceGraphUtility.SchemaNamespace)]
    public partial class SelectNodeMatchDate : INotifyPropertyChanged
    {

        [XmlAttribute(AttributeName = "StartDate")]
        public System.DateTime _StartDate;

        [XmlIgnore()]
        public bool _StartDateSpecified;

        public const string StartDateProperty = "StartDate";

        [XmlAttribute(AttributeName = "EndDate")]
        public System.DateTime _EndDate;

        [XmlIgnore()]
        public bool _EndDateSpecified;

        public const string EndDateProperty = "EndDate";

        [XmlAttribute(AttributeName = "TargetNode")]
        public string _TargetNode;

        public const string TargetNodeProperty = "TargetNode";

        [XmlAttribute(AttributeName = "Comment")]
        public string _Comment;

        public const string CommentProperty = "Comment";

        [XmlIgnore()]
        public Nullable<System.DateTime> StartDate
        {
            get
            {
                if (_StartDateSpecified)
                {
                    return this._StartDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((_StartDate != value))
                {
                    this._StartDateSpecified = value.HasValue;
                    if (value.HasValue)
                    {
                        this._StartDate = value.Value;
                    }
                    this.SendPropertyChanged("StartDate");
                }
            }
        }

        [XmlIgnore()]
        public Nullable<System.DateTime> EndDate
        {
            get
            {
                if (_EndDateSpecified)
                {
                    return this._EndDate;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if ((_EndDate != value))
                {
                    this._EndDateSpecified = value.HasValue;
                    if (value.HasValue)
                    {
                        this._EndDate = value.Value;
                    }
                    this.SendPropertyChanged("EndDate");
                }
            }
        }

        [XmlIgnore()]
        public string TargetNode
        {
            get
            {
                return this._TargetNode;
            }
            set
            {
                if ((_TargetNode != value))
                {
                    this._TargetNode = value;
                    this.SendPropertyChanged("TargetNode");
                }
            }
        }

        [XmlIgnore()]
        public string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                if ((_Comment != value))
                {
                    this._Comment = value;
                    this.SendPropertyChanged("Comment");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanged(string propertyName)
        {
            if ((PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
