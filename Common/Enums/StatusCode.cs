using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMSV
{
    /// <summary>
    /// RFC SIP, Part: 21.4
    /// </summary>
    public enum StatusCode : int
    {
        // [Provisional]
        // Informational , provisional responses, A provisional response is response that tells to its 
        // recipient that the associated request was received but result of the processing is not 
        // known yet. Provisional responses are sent only when the processing doesn't finish immediately. 
        // The sender must stop retransmitting the request upon reception of a provisional response.
        Trying = 100,           //Extended search being performed may take a significant time so a forking proxy must send a 100 Trying response
        Ringing = 180,          //Destination user agent received INVITE, and is alerting user of call.
        CallForwarded = 181,    //Servers can optionally send this response to indicate a call is being forwarded
        Queued = 182,           //Indicates that the destination was temporarily unavailable, so the server has queued the call until the destination is available. A server may send multiple 182 responses to update progress of the queue
        SessionProgress = 183,  //This response may be used to send extra information for a call which is still being set up
        EarlyDialogTerminated = 199,    //Can be used by User Agent Server to indicate to upstream SIP entities (including the User Agent Client (UAC)) that an early dialog has been terminated

        // [Success]
        // Success, responses are positive final responses. A final response is the ultimate 
        // response that the originator of the request will ever receive. Therefore final responses 
        // express result of the processing of the associated request. Final responses also terminate 
        // transactionsi. Responses with code from 200 to 299 are positive responses that means that 
        // the request was processed successfully and accepted. For instance a 200 OK response is sent 
        // when a user accepts invitation to a session (INVITE request).
        Ok = 200,               //Indicates the request was successful.
        Accept = 202,           //Indicates that the request has been accepted for processing, but the processing has not been completed.[5]:§7.3.1[6] Deprecated.[7]:§8.3.1[3]
        NoNotification = 204,   //Indicates the request was successful, but the corresponding response will not be received.[

        // [Redirection]
        // Redirection, responses are used to redirect a caller. A redirection response gives 
        // information about the user's new location or an alternative service that the caller might 
        // use to satisfy the call. Redirection responses are usually sent by proxy servers. When 
        // a proxy receives a request and doesn't want or can't process it for any reason, it will 
        // send a redirection response to the caller and put another location into the response which 
        // the caller might want to try. It can be the location of another proxy or the current 
        // location of the callee (from the location database created by a registrar). The caller 
        // is then supposed to re-send the request to the new location. 3xx responses are final.
        MultipleChoices = 300,
        MovedPerm = 301,
        MovedTemp = 302,
        UseProxy = 305,
        AlternativeServ = 380,

        // [RequestFailure]
        // Request Failure a 4xx response means that the problem is on the sender's side. The request couldn't be processed because it contains bad syntax or cannot be fulfilled at that server.
        BadRequest = 400,//The request could not be understood due to malformed syntax.[1]:§21.4.1
        Unauthorized = 401,//The request requires user authentication. This response is issued by UASs and registrars.[1]:§21.4.2
        PaymentRequired = 402,//Reserved for future use.[1]:§21.4.3
        Forbidden = 403,//The server understood the request, but is refusing to fulfill it.[1]:§21.4.4
        NotFound = 404,//The server has definitive information that the user does not exist at the domain specified in the Request-URI. This status is also returned if the domain in the Request-URI does not match any of the domains handled by the recipient of the request.[1]:§21.4.5
        MethodNotAllowed = 405,//The method specified in the Request-Line is understood, but not allowed for the address identified by the Request-URI.[1]:§21.4.6
        NotAcceptableRequest = 406,//The resource identified by the request is only capable of generating response entities that have content characteristics but not acceptable according to the Accept header field sent in the request.
        ProxyAuthenticationRequired = 407,//The request requires user authentication. This response is issued by proxys.[1]:§21.4.8
        RequestTimeout = 408,//Couldn't find the user in time.[1]:§21.4.9
        Conflict = 409,//User already registered.[9]:§7.4.10 Deprecated by omission from later RFCs[1] and by non-registration with the IANA.[3]
        Gone = 410,//The user existed once, but is not available here any more.[1]:§21.4.10
        LengthRequired = 411,//The server will not accept the request without a valid Content-Length.[9]:§7.4.12 Deprecated by omission from later RFCs[1] and by non-registration with the IANA.[3]
        ConditionalRequestFailed = 412,//The given precondition has not been met.[10]
        RequestEntityTooLarge = 413,//Request body too large.[1]:§21.4.11
        RequestUriTooLong = 414,//The server is refusing to service the request because the Request-URI is longer than the server is willing to interpret.[1]:§21.4.12
        UnsupportedMediaType = 415,//Request body in a format not supported.[1]:§21.4.13
        UnsupportedUriScheme = 416,//Request-URI is unknown to the server.[1]:§21.4.14
        UnknownResourcePriority = 417,//There was a resource-priority option tag, but no Resource-Priority header.[11]
        BadExtension = 420,//Bad SIP Protocol Extension used, not understood by the server.[1]:§21.4.15
        ExtensionRequired = 421,//The server needs a specific extension not listed in the Supported header.[1]:§21.4.16
        SessionIntervalTooSmall = 422,//The received request contains a Session-Expires header field with a duration below the minimum timer.[12]
        IntervalTooBrief = 423,//Expiration time of the resource is too short.[1]:§21.4.17
        BadLocationInformation = 424,//The request's location content was malformed or otherwise unsatisfactory.[13]
        UseIdentityHeader = 428,//The server policy requires an Identity header, and one has not been provided.[14]:p11
        ProvideReferrerIdentity = 429,//The server did not receive a valid Referred-By token on the request.[15]
        FlowFailed = 430,//A specific flow to a user agent has failed, although other flows may succeed. This response is intended for use between proxy devices, and should not be seen by an endpoint (and if it is seen by one, should be treated as a 400 Bad Request response).[16]:§11.5
        AnonymityDisallowed = 433,//The request has been rejected because it was anonymous.[17]
        BadIdentityInfo = 436,//The request has an Identity-Info header, and the URI scheme in that header cannot be dereferenced.[14]:p11
        UnsupportedCertificate = 437,//The server was unable to validate a certificate for the domain that signed the request.[14]:p11
        InvalidIdentityHeader = 438,//The server obtained a valid certificate that the request claimed was used to sign the request, but was unable to verify that signature.[14]:p12
        FirstHopLacksOutboundSupport = 439,//The first outbound proxy the user is attempting to register through does not support the "outbound" feature of RFC 5626, although the registrar does.[16]:§11.6
        ConsentNeeded = 470,//The source of the request did not have the permission of the recipient to make such a request.[18]
        TemporarilyUnavailable = 480,//Callee currently unavailable.[1]:§21.4.18
        TransactionDoesNotExist = 481,//Server received a request that does not match any dialog or transaction.[1]:§21.4.19
        LoopDetected = 482,//Server has detected a loop.[1]:§21.4.20
        TooManyHops = 483,//Max-Forwards header has reached the value '0'.[1]:§21.4.21
        Unknown_1 = 484,//Request-URI incomplete. (AddressIncomplete)
        Ambiguous = 485,//Request-URI is ambiguous.[1]:§21.4.23
        Busy_Here = 486,//Callee is busy.[1]:§21.4.24
        RequestTerminated = 487,//Request has terminated by bye or cancel.[1]:§21.4.25
        NotAcceptableHere = 488,//Some aspects of the session description of the Request-URI is not acceptable.[1]:§21.4.26
        BadEvent = 489,//The server did not understand an event package specified in an Event header field.[5]:§7.3.2[7]:§8.3.2
        RequestPending = 491,//Server has some pending request from the same dialog.[1]:§21.4.27
        Undecipherable = 493,//Request contains an encrypted MIME body, which recipient can not decrypt.[1]:§21.4.28
        SecurityAgreementRequired = 494,//The server has received a request that requires a negotiated security mechanism, and the response contains a list of suitable security mechanisms for the requester to choose between,[19]:§§2.3.1–2.3.2 or a digest authentication challenge.[19]:§2.4
        DoNotDisturb = 490,//Callee currently is in dnd state.

        // [ServerFailure]
        // Server Failure, means that the problem is on server's side. The request is apparently valid but the server failed to fulfill it. Clients should usually retry the request later.
        ServerError = 500,          //The server could not fulfill the request due to some unexpected condition
        NotImplemented = 501,       //The server does not have the ability to fulfill the request, such as because it does not recognize the request method. (Compare with 405 Method Not Allowed, where the server recognizes the method but does not allow or support it
        BadGateway = 502,           //The server is acting as a gateway or proxy, and received an invalid response from a downstream server while attempting to fulfill the request.
        ServiceUnavailable = 503,   //The server is undergoing maintenance or is temporarily overloaded and so cannot process the request. A "Retry-After" header field may specify when the client may reattempt its request
        Timeout = 504,              //The server attempted to access another server in attempting to process the request, and did not receive a prompt response.[1]:§21.5.5
        VersionNotSupported = 505,  //The SIP protocol version in the request is not supported by the server.
        MessageTooLarge = 513,      //The request message length is longer than the server can process.
        PreconditionFailure = 580,  //The server is unable or unwilling to meet some constraints specified in the offer.

        // [GlobalFailure]
        // Global Failure, reply code means that the request cannot be fulfilled at any server. This response is usually sent by a server that has definitive information about a particular user. User agents usually send a 603 Decline response when the user doesn't want to participate in the session. 
        BusyEveryWhere = 600,       //All possible destinations are busy. Unlike the 486 response, this response indicates the destination knows there are no alternative destinations (such as a voicemail server) able to accept the call.
        Decline = 603,              //The destination does not wish to participate in the call, or cannot do so, and additionally the client knows there are no alternative destinations (such as a voicemail server) willing to accept the call.
        DoesntExist = 604,          //The server has authoritative information that the requested user does not exist anywhere.
        NotAcceptable = 606,        //The user's agent was contacted successfully but some aspects of the session description such as the requested media, bandwidth, or addressing style were not acceptable.
    }
}
