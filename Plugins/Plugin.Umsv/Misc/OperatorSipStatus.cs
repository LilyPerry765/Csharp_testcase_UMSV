using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMSV.Schema;
using System.Net;

namespace UMSV
{
    class OperatorSipStatus
    {
        public string Username;
        public string Fullname;
        public SipAccountStatus Status;
        public string CallerID;
        public DateTime? CallTime;
        public IPEndPoint EndPoint;
        public DateTime? RegisterTime;
        public string Group;
    }
}
