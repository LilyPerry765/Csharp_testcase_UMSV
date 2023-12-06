using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Management;
using UMSV.Schema;
using Folder;
using System.Text.RegularExpressions;

namespace UMSV
{
    public partial class SipServer
    {
        void HandleCustom(SipMessage message)
        {

            switch (message.Subject)
            {
                case "Dialogs":
                    var xml = MyXmlSerializer.Serialize(Dialogs);
                    sipNet.Send("INFO D" + xml, message.Sender);
                    break;

                case "SipAccounts":
                    string data = MyXmlSerializer.Serialize(Config.Default.SipAccounts);
                    sipNet.Send("INFO S" + data, message.Sender);
                    break;
            }
        }
    }
}

