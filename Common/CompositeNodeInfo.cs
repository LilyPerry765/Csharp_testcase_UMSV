using System;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using System.Windows.Data;
using UMSV.Schema;
using System.Windows.Media.Imaging;
using System.Reflection;
using Enterprise;

namespace UMSV
{
    public class CompositeNodeInfo
    {
        private Type nodeType;
        public CompositeNodeInfo(Type nodeType)
        {
            this.nodeType = nodeType;
        }

        public CompositeNodeInfo(UserControl ui)
            : this(ui.GetType())
        {
            this.ui = ui;
        }

        public Image Image
        {
            get
            {
                string path = Attribute.Icon;
                string uriString = string.Format("pack://application:,,,/{0};component/{1}", StorageAssembly.GetName().Name, path);
                return new Image()
                {
                    Source = new BitmapImage(new Uri(uriString))
                };
            }
        }

        public string Title
        {
            get
            {
                return Attribute.Title ?? GraphNode.Description;
            }
        }

        public CompositeNodeAttribute Attribute
        {
            get
            {
                return nodeType.GetCustomAttributes(typeof(CompositeNodeAttribute), true)[0] as CompositeNodeAttribute;
            }
        }

        private UserControl ui;
        public UserControl UI
        {
            get
            {
                if (ui == null)
                {
                    ui = NewUiInstance();
                }
                return ui;
            }
        }

        private NodeGroup graphNode;
        public NodeGroup GraphNode
        {
            get
            {
                if (graphNode == null)
                {
                    XmlDocument doc = (UI.FindResource("Group") as XmlDataProvider).Document;
                    graphNode = VoiceGraphUtility.Deserialize<NodeGroup>(doc.InnerXml);
                }
                return graphNode;
            }
        }

        public UserControl NewUiInstance()
        {
            return NewUiInstance(nodeType);
        }

        public static UserControl NewUiInstance(Type nodeType)
        {
            return nodeType.GetConstructor(new Type[] { }).Invoke(null) as UserControl;
        }

        private static Assembly storageAssembly;
        private static Assembly StorageAssembly
        {
            get
            {
                if (storageAssembly == null)
                {
                    storageAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assm => assm.GetName().Name == Constants.CompositeNodesAssemblyName);
                    Logger.WriteIf(storageAssembly == null, LogType.Critical, "\"{0}\" is not loaded in current AppDomain!", Constants.CompositeNodesAssemblyName);
                }
                return storageAssembly;
            }
        }

        public static bool CheckCompositeNodeAttribute(Type type, bool checkAccess)
        {
            var attributes = type.GetCustomAttributes(typeof(CompositeNodeAttribute), true);
            if (attributes == null || attributes.Length == 0)
                return false;

            if (!checkAccess)
                return true;

            var role = (attributes[0] as CompositeNodeAttribute).ViewRole;
            return string.IsNullOrEmpty(role) || Folder.User.IsInRole(Guid.Parse(role));
        }

        public static CompositeNodeInfo[] GetAll(bool checkAccess)
        {
            var types = from item in StorageAssembly.GetTypes()
                        where CheckCompositeNodeAttribute(item, checkAccess)
                        select new CompositeNodeInfo(item);

            return types.ToArray();
        }

        public static CompositeNodeInfo FindByTag(string tag)
        {
            return CompositeNodeInfo.GetAll(false).SingleOrDefault(c => c.Attribute.Tag == tag);
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", GetType().Name, Attribute.Tag);
        }
    }
}
