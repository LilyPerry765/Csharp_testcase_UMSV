using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    class DialogNotFoundException : ApplicationException
    {
        public DialogNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {

        }
    }
}
