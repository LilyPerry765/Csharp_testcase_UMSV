using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    public enum SipMethod
    {
        INVITE,         //  Indicates a client is being invited to participate in a call session.	        RFC 3261
        ACK,            //  Confirms that the client has received a final response to an INVITE request.	RFC 3261
        BYE,            //  Terminates a call and can be sent by either the caller or the callee.	        RFC 3261
        CANCEL,         //  Cancels any pending request.	                                                RFC 3261
        OPTIONS,        //  Queries the capabilities of servers.	                                        RFC 3261
        REGISTER,       //  Registers the address listed in the To header field with a SIP server.	        RFC 3261
        INFO,           //  Sends mid-session information that does not modify the session state.	        RFC 6086
        COMET,          //  precondition met
        PRACK,          //  Provisional acknowledgement.	                                                RFC 3262
        SUBSCRIBE,      //  Subscribes for an Event of Notification from the Notifier.	                    RFC 3265
        NOTIFY,         //  Notify the subscriber of a new Event.	                                        RFC 3265
        REFER,          //  Asks recipient to issue SIP request (call transfer.)	                        RFC 3515
        UPDATE,         //  Modifies the state of a session without changing the state of the dialog.	    RFC 3311
        MESSAGE,        //  Transports instant messages using SIP.	                                        RFC 3428
        PUBLISH,        //  Publishes an event to the Server.	                                            RFC 3903
    }
}
