using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMSV.Schema;

namespace UMSV
{
    public class DivertTarget
    {
        public DivertTarget(SipAccount account, string phone)
        {
            this.Account = account;
            this.Phone = phone;
        }

        public SipAccount Account;
        public string Phone;
    }
}
