using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using Enterprise;
using System.Runtime.Serialization;
using UMSV.Schema;

namespace UMSV
{
    internal partial class SipMessageConstants
    {
        public const string RegexPacketRequest = @"(?<Method>(INVITE|ACK|BYE|CANCEL|OPTIONS|REGISTER|INFO|COMET|PRACK|SUBSCRIBE|NOTIFY|REFER|FOLDER|CUSTOM|UPDATE|MESSAGE))\s+(?<Uri>.+)\s+SIP/2.0";
        public const string RegexPacketRequest_Method = "Method";
        public const string RegexPacketRequest_Uri = "Uri";
        public const string RegexPacketStatus = @"SIP/2.0\s*(?<StatusCode>\d+)\s*(?<Reason>.+)";
        public const string RegexPacketStatus_StatusCode = "StatusCode";

        public const string RegexMessageFunctionResult = @"(?<Error>\w+) '(?<Content>.*)'";
        public const string RegexMessageFunctionResult_Error = "Error";
        public const string RegexMessageFunctionResult_Content = "Content";

        public const string RegexMessageField = @"(?<Field>.*): (?<Value>.*)";
        public const string RegexMessageField_Field = "Field";
        public const string RegexMessageField_Value = "Value";

        public const string RegexSipUri = @"sip:@?((?<UserID>.+)@)?(?<Address>\d+\.\d+\.\d+\.\d+)(:(?<Port>\d+))?(;rinstance=(?<Rinstance>\w+))?";
        public const string RegexSipUri_UserID = "UserID";
        public const string RegexSipUri_Address = "Address";
        public const string RegexSipUri_Port = "Port";
        public const string RegexSipUri_Rinstance = "Rinstance";

        public const string RegexReferTo = @"\<?(?<Uri>.+)\>?";
        public const string RegexReferTo_Uri = "Uri";

        public const string RegexMessageField_From = @"(\""(?<DisplayName>.+)\"")?\s*\<(?<Uri>.+)\>\s*(;tag=(?<Tag>.+))?";
        public const string RegexMessageField_From_DisplayName = "DisplayName";
        public const string RegexMessageField_From_Uri = "Uri";
        public const string RegexMessageField_From_Tag = "Tag";

        public const string RegexMessageField_Contact = @"(\""(?<DisplayName>.+)\"")?\s*\<(?<Uri>.+)\>\s*(;tag=(?<Tag>.+))?(;expires=(?<Expires>.+))?";
        public const string RegexMessageField_Contact_DisplayName = "DisplayName";
        public const string RegexMessageField_Contact_Uri = "Uri";
        public const string RegexMessageField_Contact_Tag = "Tag";
        public const string RegexMessageField_Contact_Expires = "Expires";

        public const string RegexMessageField_Via = @"SIP/2.0/UDP\s*(?<Address>[\.\d]+)(:(?<Port>\d+))?";
        public const string RegexMessageField_Via_Address = "Address";
        public const string RegexMessageField_Via_Port = "Port";
        public const string RegexMessageField_Branch = @";branch=(?<Branch>[^;]+)";
        public const string RegexMessageField_Rport = @";rport=?(?<Rport>\w+)";

        public const string RegexMessageField_Reason = @"Q.850\s*;\s*cause\s*=\s*(?<Cause>\d+)";
        public const string RegexMessageField_Reason_Cause = "Cause";

        public const string RegexMessageField_WwwAuthenticate = @"realm=""(?<Realm>.+)""\s*,\s*nonce=""(?<Nonce>.+)""";
        public const string RegexMessageField_WwwAuthenticate_Realm = "Realm";
        public const string RegexMessageField_WwwAuthenticate_Nonce = "Nonce";

        public const string RegexMessageField_CSeq = @"(?<Number>\d+)\s+(?<Method>\w+)";
        public const string RegexMessageField_CSeq_Number = "Number";
        public const string RegexMessageField_CSeq_Method = "Method";

        public const string RegexMessageField_UserAgent = @"User-Agent:(.+)";

        public const string RegexSdpField = @"(?<FieldName>\w+)=(?<Value>.+)";
        public const string RegexSdpField_FieldName = @"FieldName";
        public const string RegexSdpField_Value = @"Value";

        public const string RegexSdpClientAddress = @".*IP4 (?<Value>.+)";
        public const string RegexSdpClientAddress_Value = @"Value";

        public const string RegexSdpMedia = @"(?<MediaType>\w+)\s+(?<Port>\d+)\s+(?<Extra>.+)";
    }

    public class SipMessage : ICloneable
    {
        public SipMessage()
        {
        }

        public SipMessage(string content)
        {
            this.Content = content;
        }

        public string Content
        {
            get
            {
                try
                {
                    string sdpPart = string.Join("\r\n", SdpFields.Select(field => field.Content).ToArray()) + "\r\n";
                    string notifyStatusCodePart = !NotifyStatusCode.HasValue ? "" : string.Format("SIP/2.0 {0} {1}\r\n\r\n", (int)NotifyStatusCode.Value, NotifyStatusCode.ToString());
                    var contentLength = sdpPart.Length + notifyStatusCodePart.Length;
                    if (contentLength <= 4)
                        contentLength = 0;
                    ContentLength.Value = contentLength.ToString();
                    string headerFields = string.Join("\r\n", HeaderFields.Select(field => field.Content).ToArray());

                    return string.Format("{0}\r\n{1}\r\n\r\n{2}{3}", HeaderFirstLine.Content, headerFields, sdpPart, notifyStatusCodePart);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    return null;
                }
            }
            set
            {
                if (value.Trim() == string.Empty)
                    return;

                var lines = value.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
                int sdpStartIndex = lines.ToList().IndexOf(string.Empty);
                if (sdpStartIndex == -1)
                    return;

                HeaderFirstLine = SipHeaderFirstLine.Deserialize(lines.First());
                lines.RemoveAt(0);

                HeaderFields = SipHeaderField.Deserialize(lines.Take(sdpStartIndex - 1));

                try
                {
                    if (HeaderFirstLine.RequestHeader != null && HeaderFirstLine.RequestHeader.Method == SipMethod.NOTIFY)
                    {
                        var match = Regex.Match(lines[sdpStartIndex], SipMessageConstants.RegexPacketStatus, RegexOptions.Compiled);
                        var val = match.Groups[SipMessageConstants.RegexPacketStatus_StatusCode].Value.Trim();
                        if (val != string.Empty)
                            NotifyStatusCode = (StatusCode)int.Parse(val);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Write(LogType.Exception, "Error handling notify message: {0}", ex.Message);
                }

                if (ContentLength.Value != "0")
                    SdpFields = SipSdpField.Deserialize(lines.Skip(sdpStartIndex));
            }
        }

        public SipHeaderFirstLine HeaderFirstLine = new SipHeaderFirstLine();
        public List<SipHeaderField> HeaderFields = new List<SipHeaderField>();
        public List<SipSdpField> SdpFields = new List<SipSdpField>();

        public IPEndPoint Sender = null;

        T GetField<T>()
            where T : SipHeaderField
        {
            return (T)this.HeaderFields.FirstOrDefault(h => h.GetType().FullName == typeof(T).FullName);
        }

        T CreateOrGetField<T>()
            where T : SipHeaderField, new()
        {
            var field = (T)this.HeaderFields.FirstOrDefault(h => h.GetType().FullName == typeof(T).FullName);
            if (field == null)
            {
                field = new T();
                this.HeaderFields.Add(field);
            }
            return field;
        }

        string GetFieldValue<T>()
            where T : SipHeaderField
        {
            var field = (T)this.HeaderFields.FirstOrDefault(h => h.GetType().FullName == typeof(T).FullName);
            if (field == null)
                return null;
            else
                return field.Value;
        }

        void SetFieldValue<T>(string value)
            where T : SipHeaderField
        {
            var field = (T)this.HeaderFields.FirstOrDefault(h => h.GetType().FullName == typeof(T).FullName);
            if (field != null)
                field.Value = value;
        }

        T GetSdpField<T>()
            where T : SipSdpField
        {
            return (T)this.SdpFields.FirstOrDefault(h => h is T);
        }

        public void AppendRecordRoute()
        {
            HeaderFields.Add(new SipFieldRecordRoute()
            {
                Value = string.Format("<{0};ftag={1};lr=on>", To.Uri.Value, From.Tag),
            });
        }

        public void PushVia(string branch)
        {
            HeaderFields.Insert(0, new SipFieldVia()
            {
                EndPoint = Config.Default.SipProxyEndPoint,
                Branch = branch,
            });
        }

        public void PopVia(string branch)
        {
            var via = Vias.FirstOrDefault(v => v.Branch == branch);
            HeaderFields.Remove(via);
        }

        public void ChangeAsResponse(StatusCode status)
        {
            HeaderFirstLine.RequestHeader = null;
            HeaderFirstLine.ResponseHeader = new SipResponseHeader(status);
        }

        public void ChangeAsRequest(SipMethod method)
        {
            HeaderFirstLine.RequestHeader = new SipRequestHeader()
            {
                Method = method,
            };
            HeaderFirstLine.ResponseHeader = null;
        }

        public void RemoveField(SipHeaderField field)
        {
            if (field != null)
                HeaderFields.Remove(field);
        }

        public void ClearSdp()
        {
            this.ContentLength.Value = "0";
            this.RemoveField(this.ContentType);
            this.SdpFields.Clear();
        }

        public StatusCode? NotifyStatusCode;

        #region Helpers

        public SipFieldCSeq CSeq
        {
            get
            {
                return GetField<SipFieldCSeq>();
            }
        }

        public SipFieldSupported Supported
        {
            get
            {
                return GetField<SipFieldSupported>();
            }
        }

        public SipFieldReason Reason
        {
            get
            {
                return GetField<SipFieldReason>();
            }
        }

        public SipFieldContentType ContentType
        {
            get
            {
                return GetField<SipFieldContentType>();
            }
        }

        public SipFieldContentLength ContentLength
        {
            get
            {
                return CreateOrGetField<SipFieldContentLength>();
            }
        }

        public string CallID
        {
            get
            {
                return GetField<SipFieldCallID>().Value;
            }
        }

        public string Subject
        {
            get
            {
                return GetFieldValue<SipFieldSubject>();
            }
        }

        //public string Function
        //{
        //    get
        //    {
        //        return GetFieldValue<SipFieldFunction>();
        //    }
        //    set
        //    {
        //        var function = GetField<SipFieldFunction>();
        //        if (function != null)
        //            function.Value = value;
        //        else
        //            this.HeaderFields.Add(new SipFieldFunction() {
        //                Value = value
        //            });
        //    }
        //}

        //public SipFieldFunctionResult FunctionResult
        //{
        //    get
        //    {
        //        return GetField<SipFieldFunctionResult>();
        //    }
        //}

        //public string FunctionParams
        //{
        //    get
        //    {
        //        return GetFieldValue<SipFieldFunctionParams>();
        //    }
        //}

        public SipFieldVia Via
        {
            get
            {
                return GetField<SipFieldVia>();
            }
        }

        public List<SipFieldVia> Vias
        {
            get
            {
                return this.HeaderFields.Where(h => h.GetType().FullName == typeof(SipFieldVia).FullName).Select(f => (SipFieldVia)f).ToList();
            }
        }

        public SipFieldFrom From
        {
            get
            {
                return GetField<SipFieldFrom>();
            }
        }

        public SipFieldContact Contact
        {
            get
            {
                return GetField<SipFieldContact>();
            }
        }

        public SipFieldUserAgent UserAgent
        {
            get
            {
                return GetField<SipFieldUserAgent>();
            }
        }

        public SipFieldExpires Expires
        {
            get
            {
                return GetField<SipFieldExpires>();
            }
        }

        public SipFieldTo To
        {
            get
            {
                return GetField<SipFieldTo>();
            }
        }

        public SipFieldReferTo ReferTo
        {
            get
            {
                return GetField<SipFieldReferTo>();
            }
        }

        public SipFieldRecordRoute RecordRoute
        {
            get
            {
                return GetField<SipFieldRecordRoute>();
            }
        }

        public SipFieldRoute Route
        {
            get
            {
                return GetField<SipFieldRoute>();
            }
        }

        public IPEndPoint RtpEndPoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse(RtpAddress), RtpPort);
            }
        }

        public string RtpAddress
        {
            get
            {
                return GetSdpField<SdpFieldConnectionInfo>().ClientAddress;
            }
            set
            {
                GetSdpField<SdpFieldConnectionInfo>().ClientAddress = value;
            }
        }

        public SdpFieldSessionOrigin SdpFieldSessionOrigin
        {
            get
            {
                return GetSdpField<SdpFieldSessionOrigin>();
            }
        }

        public SdpFieldConnectionInfo SdpFieldConnectionInfo
        {
            get
            {
                return GetSdpField<SdpFieldConnectionInfo>();
            }
        }

        public int RtpPort
        {
            get
            {
                return GetSdpField<SdpFieldMedia>().ClientPort;
            }
            set
            {
                GetSdpField<SdpFieldMedia>().ClientPort = value;
            }
        }

        public SdpFieldMedia Media
        {
            get
            {
                return GetSdpField<SdpFieldMedia>();
            }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new SipMessage()
            {
                Content = this.Content
            };
        }

        #endregion

        public SipFieldReferredBy ReferredBy
        {
            get
            {
                return GetField<SipFieldReferredBy>();
            }
        }
    }

    public class SipHeaderFirstLine
    {
        public SipRequestHeader RequestHeader;
        public SipResponseHeader ResponseHeader;

        public string Content
        {
            get
            {
                if (RequestHeader != null)
                    return RequestHeader.Content;
                else
                    return ResponseHeader.Content;
            }
            set
            {
                RequestHeader = SipRequestHeader.Deserialize(value);
                ResponseHeader = SipResponseHeader.Deserialize(value);
            }
        }

        public static SipHeaderFirstLine Deserialize(string sipHeaderFirstLine)
        {
            SipHeaderFirstLine firstLine = new SipHeaderFirstLine();
            firstLine.RequestHeader = SipRequestHeader.Deserialize(sipHeaderFirstLine);
            firstLine.ResponseHeader = SipResponseHeader.Deserialize(sipHeaderFirstLine);
            return firstLine;
        }
    }

    public class SipResponseHeader
    {
        public SipResponseHeader(StatusCode statusCode)
        {
            this.StatusCode = statusCode;
        }

        public SipResponseHeader()
        {
        }

        public StatusCode? StatusCode;

        public string Content
        {
            get
            {
                return string.Format("SIP/2.0 {0} {1}", (int)StatusCode, StatusCode.ToString().Replace("_", " "));
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexPacketStatus);
                if (match.Success)
                    StatusCode = (StatusCode)int.Parse(match.Groups[SipMessageConstants.RegexPacketStatus_StatusCode].Value);
                else
                    StatusCode = null;
            }
        }

        public static SipResponseHeader Deserialize(string firstLineContent)
        {
            var header = new SipResponseHeader()
            {
                Content = firstLineContent
            };

            return header.StatusCode.HasValue ? header : null;
        }
    }

    public class SipRequestHeader
    {
        public SipMethod Method;
        public SipUri MethodUri;

        public string Content
        {
            get
            {
                return string.Format("{0} {1} SIP/2.0", Method, MethodUri.ToString());
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexPacketRequest);
                if (match.Success)
                {
                    Method = (SipMethod)Enum.Parse(typeof(SipMethod), match.Groups[SipMessageConstants.RegexPacketRequest_Method].Value, true);
                    MethodUri = new SipUri()
                    {
                        Value = match.Groups[SipMessageConstants.RegexPacketRequest_Uri].Value
                    };
                }
                else
                    MethodUri = null;
            }
        }

        public static SipRequestHeader Deserialize(string firstLineContent)
        {
            SipRequestHeader header = new SipRequestHeader()
            {
                Content = firstLineContent
            };
            if (header.MethodUri == null)
                return null;
            else
                return header;
        }
    }

    public class SipFieldUserAgent : SipHeaderField
    {
        public const string PendarUserAgentDefaultValue = "PendarSip";
        public const string CallCenterUserAgentDefaultValue = "CallCenter";
        public const string CiscoUserAgentPrefix = "Cisco-SIPGateway";

        private string AgentValue = string.Empty;
        private bool isAgentValid = false;

        public override string Value
        {
            get
            {
                return AgentValue;
            }
            set
            {
                isAgentValid = value.StartsWith(PendarUserAgentDefaultValue) ||
                               value.StartsWith(CallCenterUserAgentDefaultValue) ||
                               value.StartsWith(CiscoUserAgentPrefix);
                AgentValue = value;
            }
        }

        public bool IsAgentValid
        {
            get
            {
                return isAgentValid;
            }
        }
    }

    public class SipFieldCallID : SipHeaderField
    {
    }

    public class SipFieldMaxForwards : SipHeaderField
    {
    }

    public class SipFieldProxyAuthorization : SipHeaderField
    {
    }

    public class SipFieldWwwAuthenticate : SipHeaderField
    {
        public string Realm;
        public string Nonce;

        public override string Value
        {
            get
            {
                return string.Format("Digest realm=\"{0}\", nonce=\"{1}\"", Realm, Nonce);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexMessageField_WwwAuthenticate);
                if (!match.Success)
                    throw new FormatException("Invalid WwwAuthenticate Format: " + value);

                Realm = match.Groups[SipMessageConstants.RegexMessageField_WwwAuthenticate_Realm].Value;
                Nonce = match.Groups[SipMessageConstants.RegexMessageField_WwwAuthenticate_Nonce].Value;
            }
        }
    }

    public class SipFieldCSeq : SipHeaderField
    {
        public int Number;
        public SipMethod Method;

        public override string Value
        {
            get
            {
                return string.Format("{0} {1}", Number, Method);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexMessageField_CSeq);
                if (!match.Success)
                    throw new FormatException("Invalid CSeq Format: " + value);

                Number = int.Parse(match.Groups[SipMessageConstants.RegexMessageField_CSeq_Number].Value);
                Method = (SipMethod)Enum.Parse(typeof(SipMethod), match.Groups[SipMessageConstants.RegexMessageField_CSeq_Method].Value, true);
            }
        }
    }

    public class SipFieldFrom : SipHeaderField
    {
        public SipUri Uri;
        public string Tag;
        public string DisplayName;

        public string UriWithDisplayName
        {
            get
            {
                return string.Format("{0}<{1}>", string.IsNullOrEmpty(DisplayName) ? "" : "\"" + DisplayName + "\"", Uri.Value);
            }
        }

        public override string Value
        {
            get
            {
                return string.Format("{0}<{1}>{2}", string.IsNullOrEmpty(DisplayName) ? "" : "\"" + DisplayName + "\"", Uri.Value,
                    string.IsNullOrEmpty(Tag) ? "" : ";tag=" + Tag);
            }
            set
            {
                Match match = Regex.Match(value, SipMessageConstants.RegexMessageField_From, RegexOptions.Compiled);
                if (!match.Success)
                    throw new FormatException("Invalid From Format: " + value);

                if (match.Groups[SipMessageConstants.RegexMessageField_From_DisplayName].Success)
                    DisplayName = match.Groups[SipMessageConstants.RegexMessageField_From_DisplayName].Value;

                if (match.Groups[SipMessageConstants.RegexMessageField_From_Tag].Success)
                    Tag = match.Groups[SipMessageConstants.RegexMessageField_From_Tag].Value;

                Uri = new SipUri()
                {
                    Value = match.Groups[SipMessageConstants.RegexMessageField_From_Uri].Value
                };
            }
        }
    }

    public class SipFieldVia : SipHeaderField
    {
        public SipFieldVia()
        {
            FieldName = "Via";
        }

        public string Address;
        public int? Port;
        public string Branch;
        public string Rport;

        public IPEndPoint EndPoint
        {
            get
            {
                return new IPEndPoint(IPAddress.Parse(Address), Port.HasValue ? Port.Value : 5060);
            }
            set
            {
                Address = value.Address.ToString();
                Port = value.Port;
            }
        }

        public override string Value
        {
            get
            {
                return string.Format("SIP/2.0/UDP {0}{1}{2}{3}",
                    Address,
                    Port.HasValue ? ":" + Port.ToString() : "",
                    string.IsNullOrEmpty(Branch) ? "" : ";branch=" + Branch,
                    string.IsNullOrEmpty(Rport) ? "" : ";rport=" + Rport);
            }
            set
            {
                Match match = Regex.Match(value, SipMessageConstants.RegexMessageField_Via, RegexOptions.Compiled);
                if (!match.Success)
                    throw new FormatException("Invalid Via Format: " + value);

                Address = match.Groups[SipMessageConstants.RegexMessageField_Via_Address].Value;
                if (match.Groups[SipMessageConstants.RegexMessageField_Via_Port].Success)
                    Port = int.Parse(match.Groups[SipMessageConstants.RegexMessageField_Via_Port].Value);

                match = Regex.Match(value, SipMessageConstants.RegexMessageField_Rport, RegexOptions.Compiled);
                if (match.Success)
                    Rport = match.Groups["Rport"].Value;
                else if (Port.HasValue)
                    Rport = Port.Value.ToString();

                match = Regex.Match(value, SipMessageConstants.RegexMessageField_Branch, RegexOptions.Compiled);
                if (match.Success)
                    Branch = match.Groups["Branch"].Value;
            }
        }
    }

    public class SipFieldContentType : SipHeaderField
    {
    }

    public class SipFieldAllow : SipHeaderField
    {
    }

    public class SipFieldRecordRoute : SipHeaderField
    {
        public SipFieldRecordRoute()
        {
            FieldName = "Record-Route";
        }
    }

    public class SipFieldRoute : SipHeaderField
    {
        public SipFieldRoute()
        {
            FieldName = "Route";
        }
    }

    public class SipFieldReplaces : SipHeaderField
    {
        public SipFieldReplaces()
        {
            FieldName = "Replaces";
        }
    }

    public class SipFieldReason : SipHeaderField
    {
        public DisconnectCause? Cause;

        public SipFieldReason()
        {
            FieldName = "Reason";
        }

        public override string Value
        {
            get
            {
                if (Cause.HasValue)
                    return string.Format("Q.850;cause={0}", (int)Cause.Value);
                else
                    return base.Value;
            }
            set
            {
                base.Value = value;

                Match match = Regex.Match(value, SipMessageConstants.RegexMessageField_Reason, RegexOptions.Compiled);
                if (match.Success)
                    Cause = (DisconnectCause)int.Parse(match.Groups[SipMessageConstants.RegexMessageField_Reason_Cause].Value);
            }
        }
    }

    public class SipFieldSubject : SipHeaderField
    {
    }

    public class SipFieldContentLength : SipHeaderField
    {
        public SipFieldContentLength()
        {
            FieldName = "Content-Length";
        }

        public int Length;

        public override string Value
        {
            get
            {
                return Length.ToString();
            }
            set
            {
                Length = int.Parse(value);
            }
        }
    }

    public class SipFieldSupported : SipHeaderField
    {
    }

    public class SipFieldExpires : SipHeaderField
    {
        public int Seconds;

        public override string Value
        {
            get
            {
                return Seconds.ToString();
            }
            set
            {
                Seconds = int.Parse(value);
            }
        }

    }

    public class SipFieldTo : SipFieldFrom
    {
    }

    public class SipFieldReferTo : SipHeaderField
    {
        public SipUri Uri;

        public override string Value
        {
            get
            {
                if (Uri == null)
                {
                    Logger.WriteError("Uri in SipFieldReferTo/SipFieldReferBy is null.");
                    return null;
                }
                else
                    return string.Format("<{0}>", Uri.Value);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexReferTo);
                if (match.Success)
                {
                    Uri = new SipUri()
                    {
                        Value = match.Groups[SipMessageConstants.RegexReferTo_Uri].Value,
                    };
                }
                else
                    throw new FormatException("Invalid ReferTo Format: " + value);
            }
        }
    }

    public class SipFieldReferredBy : SipFieldReferTo
    {
    }

    public class SipFieldContact : SipHeaderField
    {
        public SipUri Uri;
        public string Tag;
        public string DisplayName;
        public string Expires;

        public override string Value
        {
            get
            {
                return string.Format("{0}<{1}>{2}{3}", string.IsNullOrEmpty(DisplayName) ? "" : "\"" + DisplayName + "\"",
                    Uri.Value,
                    string.IsNullOrEmpty(Tag) ? "" : ";tag=" + Tag,
                    string.IsNullOrEmpty(Expires) ? "" : ";expires=" + Expires);
            }
            set
            {
                Match match = Regex.Match(value, SipMessageConstants.RegexMessageField_Contact, RegexOptions.Compiled);
                if (match.Success)
                {
                    if (match.Groups[SipMessageConstants.RegexMessageField_Contact_DisplayName].Success)
                        DisplayName = match.Groups[SipMessageConstants.RegexMessageField_Contact_DisplayName].Value;

                    if (match.Groups[SipMessageConstants.RegexMessageField_Contact_Tag].Success)
                        Tag = match.Groups[SipMessageConstants.RegexMessageField_Contact_Tag].Value;

                    if (match.Groups[SipMessageConstants.RegexMessageField_Contact_Expires].Success)
                        Expires = match.Groups[SipMessageConstants.RegexMessageField_Contact_Expires].Value;

                    Uri = new SipUri()
                    {
                        Value = match.Groups[SipMessageConstants.RegexMessageField_Contact_Uri].Value
                    };
                }
                else
                {
                    match = Regex.Match(value, SipMessageConstants.RegexSipUri, RegexOptions.Compiled);
                    if (!match.Success)
                        throw new FormatException("Invalid Contact Format: " + value);
                    else
                        Uri = new SipUri()
                        {
                            Value = match.Value
                        };
                }
            }
        }
    }

    public class SipUri
    {
        public string Value
        {
            get
            {
                return string.Format("sip:{0}{4}{1}{2}{3}", UserID, Address, Port.HasValue ? ":" + Port : "", string.IsNullOrEmpty(Rinstance) ? "" : ";rinstance=" + Rinstance,
                    (string.IsNullOrEmpty(UserID) ? "" : "@"));
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexSipUri);
                if (match.Success)
                {
                    UserID = match.Groups[SipMessageConstants.RegexSipUri_UserID].Value;
                    Address = match.Groups[SipMessageConstants.RegexSipUri_Address].Value;

                    if (match.Groups[SipMessageConstants.RegexSipUri_Port].Success)
                        Port = int.Parse(match.Groups[SipMessageConstants.RegexSipUri_Port].Value);

                    if (match.Groups[SipMessageConstants.RegexSipUri_Rinstance].Success)
                        Rinstance = match.Groups[SipMessageConstants.RegexSipUri_Rinstance].Value;
                }
                else
                    throw new FormatException("Invalid Sip Uri: " + value);
            }
        }

        public override string ToString()
        {
            return Value;
        }

        public IPEndPoint EndPoint
        {
            get
            {
                int port = Port.HasValue ? Port.Value : 5060;
                return new IPEndPoint(IPAddress.Parse(Address), port);
            }
            set
            {
                Address = value.Address.ToString();
                Port = value.Port;
            }
        }

        public string UserID;
        public string Address;
        public string Rinstance;
        public int? Port;
    }

    public class SipHeaderField
    {
        public string Content
        {
            get
            {
                return string.Format("{0}: {1}", FieldName, Value);
            }
        }

        public virtual string Value
        {
            get;
            set;
        }

        public string FieldName
        {
            get;
            set;
        }

        public static List<SipHeaderField> Deserialize(IEnumerable<string> messageLines)
        {
            List<SipHeaderField> fields = new List<SipHeaderField>();

            foreach (var messageLine in messageLines)
            {
                SipHeaderField headerField = null;
                Match match = Regex.Match(messageLine, SipMessageConstants.RegexMessageField, RegexOptions.Compiled);
                if (match.Success)
                {
                    string field = match.Groups[SipMessageConstants.RegexMessageField_Field].Value;
                    string value = match.Groups[SipMessageConstants.RegexMessageField_Value].Value.Trim();

                    switch (field.ToUpper())
                    {
                        case "FROM":
                            headerField = new SipFieldFrom();
                            break;

                        case "TO":
                            headerField = new SipFieldTo();
                            break;

                        case "REFER-TO":
                            headerField = new SipFieldReferTo();
                            break;

                        case "REFERRED-BY":
                            headerField = new SipFieldReferredBy();
                            break;

                        case "CONTACT":
                            headerField = new SipFieldContact();
                            break;

                        case "CALL-ID":
                            headerField = new SipFieldCallID();
                            break;

                        case "REPLACES":
                            headerField = new SipFieldReplaces();
                            break;

                        case "USER-AGENT":
                            headerField = new SipFieldUserAgent();
                            break;

                        case "VIA":
                            if (value.Contains(','))
                            {
                                foreach (var part in value.Split(','))
                                {
                                    fields.Add(new SipFieldVia()
                                    {
                                        Value = part
                                    });
                                }
                                continue;
                            }
                            headerField = new SipFieldVia();
                            break;

                        case "CSEQ":
                            headerField = new SipFieldCSeq();
                            break;

                        case "MAX-FORWARDS":
                            headerField = new SipFieldMaxForwards();
                            break;

                        case "EXPIRES":
                            headerField = new SipFieldExpires();
                            break;

                        case "PROXY-AUTHORIZATION":
                            headerField = new SipFieldProxyAuthorization();
                            break;

                        case "WWW-AUTHENTICATE":
                            headerField = new SipFieldWwwAuthenticate();
                            break;

                        case "ALLOW":
                            headerField = new SipFieldAllow();
                            break;

                        case "SUPPORTED":
                            headerField = new SipFieldSupported();
                            break;

                        case "CONTENT-LENGTH":
                            headerField = new SipFieldContentLength();
                            break;

                        case "RECORD-ROUTE":
                            headerField = new SipFieldRecordRoute();
                            break;

                        case "ROUTE":
                            headerField = new SipFieldRoute();
                            break;

                        case "REASON":
                            headerField = new SipFieldReason();
                            break;

                        case "CONTENT-TYPE":
                            headerField = new SipFieldContentType();
                            break;

                        case "SUBJECT":
                            headerField = new SipFieldSubject();
                            break;

                        default:
                            headerField = new SipHeaderField();
                            break;
                    }

                    headerField.FieldName = field;
                    headerField.Value = value;
                    fields.Add(headerField);
                }
                else
                {
                    Logger.WriteError("Invalid Message Field, line: '{0}'", messageLine);
                }
            }

            return fields;
        }
    }

    public class SipSdpField
    {
        public SipSdpField(string content)
        {
            this.Content = content;
        }

        public SipSdpField()
        {
        }

        public string Content
        {
            get
            {
                return string.Format("{0}={1}", FieldName, Value);
            }
            set
            {
                Match match = Regex.Match(value, SipMessageConstants.RegexSdpField, RegexOptions.Compiled);
                if (match.Success)
                {
                    FieldName = match.Groups[SipMessageConstants.RegexSdpField_FieldName].Value;
                    Value = match.Groups[SipMessageConstants.RegexSdpField_Value].Value;
                }
            }
        }

        public string FieldName
        {
            get;
            set;
        }

        public virtual string Value
        {
            get;
            set;
        }

        public static List<SipSdpField> Deserialize(IEnumerable<string> messageLines)
        {
            List<SipSdpField> fields = new List<SipSdpField>();

            foreach (var messageLine in messageLines.Where(str => !string.IsNullOrEmpty(str)))
            {
                SipSdpField sdpField = null;
                Match match = Regex.Match(messageLine, SipMessageConstants.RegexSdpField, RegexOptions.Compiled);
                if (match.Success)
                {
                    string field = match.Groups[SipMessageConstants.RegexSdpField_FieldName].Value;
                    string value = match.Groups[SipMessageConstants.RegexSdpField_Value].Value;

                    switch (field)
                    {
                        case "o":
                            match = Regex.Match(value, SipMessageConstants.RegexSdpClientAddress);
                            sdpField = new SdpFieldSessionOrigin()
                            {
                                ClientAddress = match.Groups[SipMessageConstants.RegexSdpClientAddress_Value].Value,
                            };
                            break;

                        case "c":
                            sdpField = new SdpFieldConnectionInfo();
                            break;

                        case "m":
                            sdpField = new SdpFieldMedia();
                            break;

                        case "v":
                            sdpField = new SdpFieldProtocolVersion();
                            break;

                        case "t":
                            sdpField = new SdpFieldTimeOfTheSession();
                            break;

                        default:
                            sdpField = new SipSdpField();
                            break;
                    }

                    sdpField.FieldName = field;
                    sdpField.Value = value;
                    fields.Add(sdpField);
                }
                else
                {
                    Logger.WriteError("Invalid Message SDP Field: '{0}'", messageLine);
                }
            }

            return fields;
        }
    }

    /// <example>
    /// o=...
    /// </example>
    public class SdpFieldSessionOrigin : SipSdpField
    {
        public string ClientAddress;

        public override string Value
        {
            get
            {
                return string.Format("- 3517046709 3517046709 IN IP4 {0}", ClientAddress);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexSdpClientAddress);
                ClientAddress = match.Groups[SipMessageConstants.RegexSdpClientAddress_Value].Value.Trim();
            }
        }
    }

    /// <summary>
    /// the start time and stop time for pre-arranged multicast conference
    /// </summary>
    /// <example>
    /// t=0 0
    /// </example>
    public class SdpFieldTimeOfTheSession : SipSdpField { }

    /// <example>
    /// i=...
    /// </example>
    public class SdpFieldSessionInformation : SipSdpField { }

    /// <example>
    /// u=...
    /// </example>
    public class SdpFieldUriOfDescription : SipSdpField { }

    /// <example>
    /// v=...
    /// </example>
    public class SdpFieldProtocolVersion : SipSdpField { }

    /// <example>
    /// c=IN IP4 10.172.0.115
    /// </example>
    public class SdpFieldConnectionInfo : SipSdpField
    {
        public string ClientAddress;

        public override string Value
        {
            get
            {
                return string.Format("IN IP4 {0}", ClientAddress);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexSdpClientAddress);
                ClientAddress = match.Groups[SipMessageConstants.RegexSdpClientAddress_Value].Value.Trim();
            }
        }
    }

    /// <example>
    /// m=audio 6000 RTP/AVP 0
    /// </example>
    public class SdpFieldMedia : SipSdpField
    {
        public int ClientPort;
        public MediaType Type = MediaType.Audio;
        public string Extra = "RTP/AVP 8 101";

        public override string Value
        {
            get
            {
                return string.Format("{1} {0} {2}", ClientPort, Type.ToString().ToLower(), Extra);
            }
            set
            {
                var match = Regex.Match(value, SipMessageConstants.RegexSdpMedia);
                if (!match.Success)
                    throw new FormatException("Invalid Media Format: " + value);

                Type = (MediaType)Enum.Parse(typeof(MediaType), match.Groups["MediaType"].Value, true);
                ClientPort = int.Parse(match.Groups["Port"].Value);
                Extra = match.Groups["Extra"].Value;
            }
        }

        public enum MediaType
        {
            Audio,
            Video,
            Text,
            Application,
            Control,
            Message,
            Image, // Fax
        }
    }

    /// <example>
    /// a=T38FaxUdpEC:t38UDPRedundancy
    /// a=T38FaxUdpEC:t38UDPRedundancy
    /// a=sendrecv
    /// </example>
    public class SdpFieldMediaAttribute : SipSdpField
    {
        public SdpFieldMediaAttribute(string value)
        {
            FieldName = "a";
            Value = value;
        }
    }
}
