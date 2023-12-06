﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UMSV
{
    public enum VoiceGroup
    {
        [Description("صدای پیش فرض سیستم، آقا")]
        DefaultMan = 1,

        [Description("صدای پیش فرض سیستم، خانم")]
        DefaultWoman = 2,

        [Description("صداهای خصوصی سازی شده مشتری")]
        CustomerSystemVoice = 3,

        [Description("تکریم")]
        CustomerCare = 5,

        [Description("صدا مخصوص ارسال پیام")]
        ForInforming = 6,
    }
}
