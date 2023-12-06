using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Serialization;
using Folder;

namespace UMSV.Schema
{
    partial class Config
    {
        public static Config Default;

        static Config()
        {
            Load();
        }

        public static bool Save()
        {
            try
            {
                FolderDataContext dc = new FolderDataContext();
                var file = dc.Files.Single(f => f.Name == Constants.ConfigFileName);
                file.Content = System.Text.Encoding.Unicode.GetBytes(MyXmlSerializer.Serialize(Config.Default));
                dc.SubmitChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("UMS.xml file not found or error serializing it", ex);
            }
        }

        public static bool Load()
        {
            try
            {
                FolderDataContext dc = new FolderDataContext();
                var file = dc.Files.Single(f => f.Name == Constants.ConfigFileName);
                Default = MyXmlSerializer.Deserialize<Config>(file.DataAsUnicode);

                Default.ExtractSipProxyAddress();
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("UMS.xml file not found or error deserializing it", ex);
            }
        }

        public void ExtractSipProxyAddress()
        {
            if (string.IsNullOrEmpty(Config.Default.SipProxyAddress))
            {
                var localIP = Utility.GetLocalIP();
                _SipProxyEndPoint = new IPEndPoint(localIP, Config.Default.SipProxyPort);
            }
            else
                _SipProxyEndPoint = new IPEndPoint(IPAddress.Parse(Config.Default.SipProxyAddress), Config.Default.SipProxyPort);
        }

        [XmlIgnore]
        private IPEndPoint _SipProxyEndPoint;

        [XmlIgnore]
        public IPEndPoint SipProxyEndPoint
        {
            get
            {
                return _SipProxyEndPoint;
            }
        }
    }
}
