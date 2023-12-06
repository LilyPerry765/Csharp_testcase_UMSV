using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMSV;
using UMSV.Schema;

namespace UMSV
{
    public class SipDialog
    {
        public string CallerID
        {
            get;
            set;
        }

        public string CalleeID
        {
            get;
            set;
        }

        public string DialogID
        {
            get;
            set;
        }

        public string DivertCallID
        {
            get;
            set;
        }

        public DialogStatus Status
        {
            get;
            set;
        }

        public DateTime CallTime
        {
            get;
            set;
        }
    }
}
