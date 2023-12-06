using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    public enum SipAccountStatus
    {
        Offline,
        Idle,
        Talking,
        Dialing,
        Hold,
        
        DND,
        /*
         *  the callee may not wish to reveal the DND condition to the caller.  If not, and if it cannot rely on a proxy in its domain to
            take alternative action, then a code such as 486 Busy (to make it look like a busy condition), 180 Alerting followed later by 408
            Request Timeout (to make it look like a no reply condition) or 603 Decline (to make it look like a deliberate rejection by the user)
            might be chosen.  However, the choice of SIP response code or SIP header field in this situation is outside the scope of this document.

            Assuming the callee is prepared to reveal the DND condition to the caller, one possibility is the SIP 480 Temporarily Unavailable
            response code.  RFC 3261 [1] assigns the following meaning to the 480 response code:
         * 
         */
    }
}
