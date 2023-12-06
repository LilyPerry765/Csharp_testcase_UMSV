using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UMSV.Engine
{
    public enum InformingRecordStatus : byte
    {
        [Description("منتظر")]
        Queued = 0,

        [Description("موفق")]
        Done = 1,

        [Description("ناموفق")]
        UnsuccessfulCall = 2,

        [Description("سعی برای تماس")]
        InProgress = 3,

        [Description("برقرار")]
        Connected = 4,

        [Description("ست آپ")]
        SetupProgress = 5,
    }
}
