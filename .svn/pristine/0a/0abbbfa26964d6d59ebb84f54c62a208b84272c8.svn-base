using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    public struct NumberVoice
    {
        public NumberVoice(object value, NumberSuffix? suffix)
        {
            this.Value = value.ToString();
            this.Suffix = suffix;
        }

        public NumberVoice(object value)
            : this(value, null)
        {
        }

        public string Value;
        public NumberSuffix? Suffix;
    }
}
