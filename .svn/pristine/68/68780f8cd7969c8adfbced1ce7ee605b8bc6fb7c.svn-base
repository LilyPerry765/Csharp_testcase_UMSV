using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Enterprise;
using UMSV.Schema;

namespace UMSV
{
    public partial class SipService
    {
        const string Template_Trying = "TRYING";
        const string Template_Ack = "ACK";
        const string Template_AckBusy = "ACK-2";
        const string Template_Cancel = "CANCEL";
        const string Template_Register = "REGISTER";
        const string Template_Bye = "BYE";
        const string Template_SoftphoneBye = "SOFTPHONE-BYE";
        const string Template_Invite = "INVITE";
        const string Template_SoftphoneInvite = "SOFTPHONE-INVITE";
        const string Template_Ok = "OK";
        const string Template_FAXOK = "FAXOK";
        const string Template_PASSTHROUGHFAXOK = "PASSTHROUGH-FAXOK";
        const string Template_SoftphoneOk = "SOFTPHONE-OK";
        const string Template_Refer = "REFER";
        const string Template_BusyHere = "BusyHere";
        const string Template_Notify = "NOTIFY";
        const string Template_FaxReinvite = "FAXREINVITE";
        const string Template_TransactionDoesNotExist = "TRANSACTION-DOES-NOT-EXIST";

        string SendTransactionDoesNotExist(IPEndPoint endPoint, string to, string via, string from, string callid, string cseq)
        {
            return SendTemplateMessage(Template_TransactionDoesNotExist, endPoint, to, via, from, callid, cseq);
        }

        string SendNotify(IPEndPoint endPoint, string sourceUserID, IPEndPoint sourceEndPoint, string callID, string fromTag, string toTag, int cseq, StatusCode code)
        {
            return SendTemplateMessage(Template_Notify, endPoint, sourceUserID, sourceEndPoint, callID, fromTag, toTag, cseq, (int)code, code.ToString());
        }

        string SendBye(IPEndPoint endPoint, string dialogID, string toValue, string fromValue, string routeFtag, string userID, string extension, DialogType dialogType)
        {
            //For more information refer to the comment inside SendAck Method.
            //The same scenario for BYE...
            if ((dialogType == DialogType.ClientOutgoing || dialogType == DialogType.GatewayOutgoing) && !string.IsNullOrWhiteSpace(extension))
                return SendTemplateMessage(Template_Bye, endPoint, dialogID, toValue, fromValue, routeFtag, extension);
            else
                return SendTemplateMessage(Template_Bye, endPoint, dialogID, toValue, fromValue, routeFtag, userID);
        }

        void SendAck(IPEndPoint endPoint, string callerID, string calleeID, string callID, string viaBranch, string fromTag, string to, string extension, DialogType dialogType)
        {
            /*
             * I think the way that Cisco interprets Ack message depends on it's iOS version.
             * For instance, Ack headers like 1113@x.y.z.t:5060 (which 1113 is the Cisco's account) for outcalls are accepted in 
             * one router while they are droped in another!
             * Both routers have the same configuration but different iOS; first one, iOS 12.4 (9) and second one, iOS 12.4 (5a).
             * Finally, based on RFC 3261 we changed the header of Ack message for outcalls in order to make it understandable for
             * all iOS versions.
             * For more information refer to RFC 3261, page 129, Construction of the ACK Request.
             */
            if ((dialogType == DialogType.ClientOutgoing || dialogType == DialogType.GatewayOutgoing) && !string.IsNullOrWhiteSpace(extension))
                SendTemplateMessage(Template_Ack, endPoint, callerID, extension, callID, viaBranch, fromTag, to);
            else
                SendTemplateMessage(Template_Ack, endPoint, callerID, calleeID, callID, viaBranch, fromTag, to);
        }

        string SendInvite(SipDialog dialog, string callerID, string calleeID, int rtpPort, string callID, IPEndPoint sipProxyEndPoint, string referedByUserID, string graphTrack, string replaces)
        {
            string extention = string.IsNullOrEmpty(referedByUserID) ? "" : string.Format("Referred-By: <sip:{0}@{1}>\r\nRecord-Route: <sip:{2}@{1};ftag={3};lr=on>\r\n",
                referedByUserID, Config.Default.SipProxyEndPoint, callerID, Generate32bitRandomNumber());

            if (!string.IsNullOrEmpty(replaces))
                extention += string.Format("Replaces: {0}\r\n", replaces);

            string displayName = GetCallerDisplayName(callerID);
            string message = CreateTemplateMessage(Template_Invite, callerID, calleeID, rtpPort, callID, sipProxyEndPoint, extention, graphTrack, displayName);
            dialog.InviteMessage = new SipMessage(message);

            sipNet.Send(message, dialog.ToAccount.SipEndPoint);
            return message;
        }

        private string GetCallerDisplayName(string callerID)
        {
            return callerID;
        }

        string SendRegister(IPEndPoint endPoint, int cseq, int expires, IPEndPoint localAddress, string userName, string authorizationPhrase, string contactExpiresPhrase)
        {
            return SendTemplateMessage(Template_Register, endPoint, cseq, expires, localAddress, userName, authorizationPhrase, contactExpiresPhrase);
        }

        string GenerateBranch()
        {
            return string.Format("z9hG4bK-{0}-ppco", Guid.NewGuid());
        }

        string Generate32bitRandomNumber()
        {
            Random rnd = new Random();
            var buffer = new byte[sizeof(Int32)];
            rnd.NextBytes(buffer);
            Int32 rn = BitConverter.ToInt32(buffer, 0);
            if (rn > 0)
                return rn.ToString();
            return (rn * -1).ToString();
        }

        string SendTemplateMessage(string templateName, IPEndPoint endPoint, params object[] args)
        {
            string message = CreateTemplateMessage(templateName, args);
            sipNet.Send(message, endPoint);
            return message;
        }

        string CreateTemplateMessage(string templateName, params object[] args)
        {
            var stream = this.GetType().Assembly.GetManifestResourceStream(this.GetType().Namespace + ".Template." + templateName);
            if (stream == null)
            {
                Logger.Write(LogType.Exception, "Template '{0}' does not exist in assembly!", templateName);
                return null;
            }

            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            string template = System.Text.Encoding.ASCII.GetString(buffer);

            template = template.Replace("[SipProxyEndPoint]", Config.Default.SipProxyEndPoint.ToString());
            template = template.Replace("[SipProxyAddress]", Config.Default.SipProxyEndPoint.Address.ToString());
            template = template.Replace("[SipProxyPort]", Config.Default.SipProxyPort.ToString());
            template = template.Replace("[NewBranch]", GenerateBranch());
            template = template.Replace("[RandomNumber]", Generate32bitRandomNumber());
            template = template.Replace("[NewGuid]", Guid.NewGuid().ToString());

            string message = string.Format(template, args);

            if (message.Contains("Content-Type: application/sdp") || message.Contains("Content-Type: message/sipfrag"))
            {
                int sdpLength;
                if (message.IndexOf("Content-Length: 0\r\n\r\n") > -1)
                    sdpLength = message.Length - (message.IndexOf("Content-Length: 0\r\n\r\n") + "Content-Length: 0\r\n\r\n".Length);
                else
                    sdpLength = message.Length - (message.IndexOf("\r\n\r\n") + "\r\n\r\n".Length);
                message = message.Replace("Content-Length: 0", "Content-Length: " + sdpLength);
            }

            return message;
        }
    }
}
