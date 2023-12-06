using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    public enum MailboxType : byte
    {
        [System.ComponentModel.Description("خصوصی")]
        Private = 0,

        [System.ComponentModel.Description("عمومی")]
        Public = 1
    }
}
