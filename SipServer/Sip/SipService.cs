using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using System.Net;
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Management;
using UMSV.Schema;
using Folder;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

namespace UMSV
{
    public partial class SipService
    {
        #region Constants

        const int ServiceRestartDelay = 5000; // ms
        const int WaitForEngineAckOnConnect = 2000; // ms

        const int ByeMessageMaxResendTime = 5;

        const byte VPXCC = 0x80;
        const byte M_PT_VoicePCMA = 0x08;
        const byte M_PT_Dtmf = 101;

        const int PayloadTypeOffset = 1;
        public const int RtpInterval = 20; // ms
        public const int RtpChunkSize = 160; // byte
        const int RtpHeaderLength = 12; // byte

        //Current SequenceNumber and TimeStamp 
        //byte[] SeqNum = new byte[2];
        //byte[] TimeSt = new byte[4];
        byte[] silentSamples_80 = {0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,
                                    0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,
                                    0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,
                                    0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,
                                    0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55,0x55};
        #endregion

        public SipService()
        {
            Default = this;
        }

        #region Fields

        public static SipService Default;
        public static bool IsSoftPhoneMode = false;
        public delegate bool HasGraphEventHandler(string code);
        public HasGraphEventHandler HasGraph;
        public string SoftPhoneUsername;
        public string SoftPhonePassword;
        public readonly SafeCollection<SipDialog> Dialogs = new SafeCollection<SipDialog>();
        public readonly SafeCollection<SipAccount> Accounts = new SafeCollection<SipAccount>();

        byte[] HoldVoice;
        //int HoldVoiceOffset = 0;
        Dictionary<string, byte[]> DisconnectedDialogsRecordedVoices = new Dictionary<string, byte[]>();// When dialog state is on recording-voice if dialog goes to disconnect-state this dictionary can save the recorded sound in limited time
        SipNet sipNet;// A Net instance define a local-port for all dialog SIP conversation
        System.Threading.Timer checkMessagesTimeoutsTimer;// On specified period time checks All timeouts
        System.Threading.Timer checkAccountsTimeoutsTimer;// On specified period time checks All timeouts for users registration
        System.Threading.Timer checkInviteWaitForAckDialogsTimeoutsTimer;// On specified period of time checks All timeouts for InviteWaitForAck dialogs in order to retransmit OK packet
        Multimedia.Timer rtpTimer = new Multimedia.Timer();// Tick periodic for RTP conversation

        #endregion

        string GetFriendlyName(string name)
        {
            string fiendlyName = string.Empty;
            foreach (Char chr in name)
            {
                if (Char.IsUpper(chr))
                    fiendlyName += " " + chr;
                else
                    fiendlyName += chr;
            }

            return fiendlyName;
        }

        string ComputeMd5Hash(string clearString)
        {
            Func<byte[], string> ToHex = ((pass2) => BitConverter.ToString(pass2).Replace("-", "").ToLower());
            MD5CryptoServiceProvider svc = new MD5CryptoServiceProvider();
            byte[] codedStream = svc.ComputeHash(Encoding.ASCII.GetBytes(clearString));
            return ToHex(codedStream);
        }

        #region Handle Messages

        private void HandleUnsupportedMediaType(SipMessage message)
        {
            SendAck(message);
        }

        private void SendAck(SipMessage message)
        {
            SipMessage ack = message.Clone() as SipMessage;
            ack.ClearSdp();
            ack.CSeq.Method = SipMethod.ACK;
            ack.ChangeAsRequest(SipMethod.ACK);
            ack.HeaderFirstLine.RequestHeader.MethodUri = new SipUri() { UserID = message.To.Uri.UserID, Address = Config.Default.SipProxyAddress, Port = 5060 };
            sipNet.Send(ack.Content, message.Sender);
        }

        void SendTransactionDoesNotExist(SipMessage message)
        {
            SendTransactionDoesNotExist(message.Sender, message.To.Value, message.Via.Value, message.From.Value, message.CallID, message.CSeq.Method.ToString());
        }

        void HandleBusyEveryWhere(SipMessage message)
        {
            var dialog = FindDialog(message);
            HandleFailure(message, DisconnectCause.NoCircuitChannelAvailable);
        }

        private void HandleTrying(SipMessage message)
        {
            var dialog = FindDialog(message);
            switch (dialog.Status)
            {
                case DialogStatus.Dialing:
                    dialog.Status = DialogStatus.DialingWaitForOk;
                    break;

                case DialogStatus.Invite:
                    break;

                default:
                    Logger.WriteWarning("Trying on status {0}, dialog id:{1}", dialog.Status, dialog.DialogID);
                    break;
            }
        }

        void HandleServiceUnavailable(SipMessage message)
        {
            var dialog = FindDialog(message);

            var account = Accounts.FirstOrDefault(a =>
                IsSame(a.SipEndPoint, message.Sender));

            if (account != null && account.MaxConcurrentCalls == 1)
            {
                if (!Dialogs.Any(d => d.FromAccount != null && d.FromAccount.UserID == account.UserID))
                {
                    Logger.WriteWarning("StatusCode.ServiceUnavailable on Account: {0}, no output call found on this account, go to DND", account.UserID);
                    //account.Status = SipAccountStatus.DND;
                }
                else
                {
                    Logger.WriteException("StatusCode.ServiceUnavailable on Account: {0}, output call founded on this account, status:{1}", account.UserID, account.Status);
                    if (account.Status == SipAccountStatus.Idle)
                        account.Status = SipAccountStatus.Talking;
                }
            }
            else
                Logger.WriteWarning("StatusCode.ServiceUnavailable on Unknonw account, IP:{0}", message.Sender);

            HandleFailure(message, DisconnectCause.SubscriberAbsent);
        }

        void HandleTransactionDoesNotExist(SipMessage message)
        {
            Logger.WriteInfo("HandleTransactionDoesNotExist, CallID: {0}", message.CallID);

            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == message.CallID);

            if (dialog == null)
                return;

            HandleCallDisconnected(dialog, DisconnectCause.NormalUnspecified);
        }

        /// <summary>
        /// Version 1, check this function in different situations
        /// </summary>
        /// <param name="message"></param>
        void HandleSessionProgress(SipMessage message)
        {
            var dialog = FindDialog(message);

            Logger.WriteView("SessionProgress: forwardmode:{0}, id:{1}", dialog.IsForwardMode, dialog.DialogID);
            if (dialog.IsForwardMode)
            {
                if (message.Reason != null)
                    Logger.WriteWarning("Forward mode Session progress, reason:{0}", message.Reason);

                string to = dialog.RingingTo != null ? dialog.RingingTo.Value : dialog.InviteMessage.To.Value;
                SendTemplateMessage(Template_AckBusy,
                    dialog.ToAccount.SipEndPoint,
                    dialog.InviteMessage.HeaderFirstLine.RequestHeader.MethodUri,
                    dialog.ForwardInviteViaBranch,
                    dialog.InviteMessage.From.Value,
                    dialog.DialogID,
                    to);

                message.PopVia(dialog.ForwardInviteViaBranch);
                sipNet.Send(message.Content, dialog.FromAccount.SipEndPoint);
            }
            else
            {
                //SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                //SendTemplateMessage(Template_Ack, dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Tag);

                //the following line was inside the condition before; made some problems for disconnecting call from A number in informing mode.
                dialog.InviteMessage.To.Tag = message.To.Tag;
                //if (!dialog.CallerID.StartsWith("UMS"))
                if(!dialog.IsInforming)
                {
                    dialog.Status = DialogStatus.Connect;
                    ChangeAccountStatusToTalking(dialog);

                    ////////

                    dialog.Call.AnswerTime = DateTime.Now;
                    dialog.RtpNet.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(message.RtpAddress), message.RtpPort);
                    dialog.RtpNet.OnReceive += new PacketTransmitEventHandler(RtpNet_OnReceive);
                    //if (dialog.RtpNet != null && !dialog.RtpNet.IsSocketConnected)
                    dialog.RtpNet.Start();
                    Logger.WriteInfo("Receive RTP from '{0}' started on SessionProgress message, dialog:{1}", dialog.RtpNet.RemoteEndPoint, dialog.DialogID);

                    if (dialog.DivertPartner != null)
                    {
                        dialog.Status = DialogStatus.Talking;
                        dialog.DivertPartner.Status = DialogStatus.Talking;
                        dialog.DivertPartner.DivertCallID = dialog.Call.DialogID;
                        if (dialog.DialogType == DialogType.ClientOutgoing || dialog.DialogType == DialogType.ClientIncomming)
                            dialog.DivertPartner.AgentID = dialog.DialogType == DialogType.ClientIncomming ? dialog.ToAccount.UserID : dialog.FromAccount.UserID;
                        //dialog.DivertPartner.DivertTime = DateTime.Now;
                        //dialog.DivertTime = DateTime.Now;
                    }

                    CallStablished(this, new CallStablishedEventArgs(dialog));
                }
            }
            //HandleFailure(message, DisconnectCause.UnknownProblem);
        }

        void ChangeAccountStatusToTalking(SipDialog dialog)
        {
            if (dialog.ToAccount != null)
                dialog.ToAccount.Status = SipAccountStatus.Talking;

            if (dialog.FromAccount != null)
                dialog.FromAccount.Status = SipAccountStatus.Talking;

            if (dialog.DivertPartner != null)
            {
                if (dialog.DivertPartner.ToAccount != null)
                    dialog.DivertPartner.ToAccount.Status = SipAccountStatus.Talking;

                if (dialog.DivertPartner.FromAccount != null)
                    dialog.DivertPartner.FromAccount.Status = SipAccountStatus.Talking;
            }
        }

        void HandleFailure(SipMessage message, DisconnectCause cause)
        {
            var dialog = FindDialog(message);
            if (dialog.IsForwardMode)
            {
                switch (cause)
                {
                    case DisconnectCause.SubscriberAbsent:
                    case DisconnectCause.UserBusy:
                        string to = dialog.RingingTo != null ? dialog.RingingTo.Value : dialog.InviteMessage.To.Value;
                        SendTemplateMessage(Template_AckBusy,
                            dialog.ToAccount.SipEndPoint,
                            dialog.InviteMessage.HeaderFirstLine.RequestHeader.MethodUri,
                            dialog.ForwardInviteViaBranch,
                            dialog.InviteMessage.From.Value,
                            dialog.DialogID,
                            to);

                        message.PopVia(dialog.ForwardInviteViaBranch);
                        sipNet.Send(message.Content, dialog.FromAccount.SipEndPoint);

                        dialog.Status = DialogStatus.UserBusySentWaitForAck;
                        break;

                    default:
                        Logger.WriteError("Not supported cause: {0} on HandleSessionProgress, CallID: {1}", cause, message.CallID);
                        SendAck(message);
                        break;
                }
            }
            else
            {
                //Default SendAck method used in order to handle header problems.
                //Refer to the comment in the following method.
                //Also message.To.Tag changed to message.To.Value
                SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                //SendTemplateMessage(Template_Ack, dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Tag);

                var reason = message.Reason;
                try
                {
                    if (message.HeaderFirstLine.ResponseHeader.StatusCode == StatusCode.SessionProgress && reason == null)
                    {
                        Logger.WriteWarning("SessionProgress with no reason, callid: {0}", dialog.DialogID);
                        return;
                    }
                }
                catch
                {
                    return;
                }

                if (reason != null && reason.Cause != null)
                    cause = reason.Cause.Value;

                if (dialog.DivertPartner != null && dialog.TransferFailureTime < Constants.TransferFailureMaxTime &&
                    (dialog.DivertPartner.Status == DialogStatus.DivertingWaitForTargetResponse || dialog.DivertPartner.Status == DialogStatus.WaitForDiverting))
                {
                    Logger.WriteWarning("Diverting for dialog {0} failed, master dialog: {1}, dialog.DivertPartner.Status: {2}, cause:{3}", dialog.DialogID, dialog.DivertPartner.DialogID, dialog.DivertPartner.Status, cause);
                    dialog.Call.DisconnectCause = (int)cause;
                    TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                    dialog.Disconnected(cause);
                    //DisconnectCall(dialog); // after 3 reject call, goes offline, after online when immediately calls send to client, client sends service unavailable, and call must be disconnected.
                }
                else
                {
                    HandleCallDisconnected(dialog, cause);
                }
            }
        }

        void HandleBusyHere(SipMessage message)
        {
            var dialog = FindDialog(message);

            if (dialog.ReferMessage != null)
            {
                Logger.WriteDebug("reject call on refering mode on operator: {0}, dialog:{1}", dialog.ToAccount.UserID, dialog.DialogID);
                SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);

                SendNotify(dialog.ReferMessage.Sender, dialog.ReferMessage.ReferredBy.Uri.UserID, dialog.ReferMessage.Sender,
                        dialog.ReferMessage.CallID, dialog.ReferMessage.To.Tag, dialog.ReferMessage.From.Tag, 2, StatusCode.Busy_Here);
                dialog.ReferMessage = null;
            }
            // operator rejects a call (or rarely operator becomes DND??)
            else if (dialog.DialogType == DialogType.ClientIncomming)
            {
                Logger.WriteDebug("Operator: {0} rejected the call, dialog:{1}", dialog.ToAccount.UserID, dialog.DialogID);
                SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);

                if (dialog.DivertPartner != null && dialog.TransferFailureTime < Constants.TransferFailureMaxTime)
                {
                    Logger.WriteWarning("Diverting for dialog {0} failed because of operator busy, master dialog: {1}, dialog.DivertPartner.Status: {2}", dialog.DialogID, dialog.DivertPartner.DialogID, dialog.DivertPartner.Status);
                    dialog.Call.DisconnectCause = (int)DisconnectCause.UserBusy;
                    dialog.ToAccount.LastCallTime = DateTime.Now; // to select another operator, or wait some seconds to divert again
                    dialog.TransferFailureTime++;
                    TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                    dialog.Disconnected(DisconnectCause.UserBusy);
                }
                else
                    HandleCallDisconnected(dialog, DisconnectCause.UserBusy);
            }
            else
            {
                HandleFailure(message, DisconnectCause.UserBusy);
            }

            //HandleFailure(message, DisconnectCause.UserBusy);
        }

        void HandleNotFound(SipMessage message)
        {
            // 1: شماره تلفن مقصد غلط
            HandleFailure(message, DisconnectCause.UnallocatedNumber);
        }

        void HandleTemporarilyUnavailable(SipMessage message)
        {
            var dialog = FindDialog(message);
            if (dialog.DialogType == DialogType.ClientIncomming) // forward call from gateway to client which is DND
            {
                Logger.WriteDebug("Temporarily Unavailable detected on operator: {0} with status: {2}, dialog:{1}", dialog.ToAccount.UserID, dialog.DialogID, dialog.ToAccount.Status);
                SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                //dialog.ToAccount.Status = SipAccountStatus.DND;

                if (dialog.DivertPartner != null && dialog.TransferFailureTime < Constants.TransferFailureMaxTime)
                {
                    Logger.WriteWarning("Diverting for dialog {0} failed because of Temporarily Unavailable state, master dialog: {1}, dialog.DivertPartner.Status: {2}", dialog.DialogID, dialog.DivertPartner.DialogID, dialog.DivertPartner.Status);
                    dialog.Call.DisconnectCause = (int)DisconnectCause.SubscriberAbsent;
                    dialog.ToAccount.LastCallTime = DateTime.Now; // to select another operator, or wait some seconds to divert again
                    dialog.TransferFailureTime++;
                    TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                    dialog.Disconnected(DisconnectCause.SubscriberAbsent);
                }
                else
                    HandleCallDisconnected(dialog, DisconnectCause.SubscriberAbsent);

                /*the following feature is currently removed*/
                //if (dialog.ToAccount.TemporarilyUnavailableCount >= Constants.TransferFailureMaxTime)
                //{
                //    SessionManager.OnAccountUnRegister(dialog.ToAccount);
                //    dialog.ToAccount.Status = SipAccountStatus.Offline;
                //    Logger.WriteInfo("Account {0} Unregistered. because of receiving several(={1}) temporarily unavailable packets.", dialog.ToAccount.UserID, dialog.ToAccount.TemporarilyUnavailableCount);
                //    if (UnRegistered != null)
                //        UnRegistered(this, new UnRegisteredEventArgs() { Account = dialog.ToAccount });
                //}

                //dialog.ToAccount.TemporarilyUnavailableCount++;
            }
            else
            {
                // خاموش بودن موبایل: 20
                // عدم آنتن دهی و امکان برقراری ارتباط: 18
                HandleFailure(message, DisconnectCause.NoUserResponding);
            }

            if ((dialog.AuditionTarget != null) || (dialog.DivertPartner != null && dialog.DivertPartner.AuditionTarget != null))
                DisconnectEavesdropperDialog(dialog);
        }

        void HandleSipRequest(SipMessage message)
        {
            try
            {
                switch (message.HeaderFirstLine.RequestHeader.Method)
                {
                    case SipMethod.INVITE:
                        HandleInvite(message);
                        break;

                    case SipMethod.REFER:
                        HandleRefer(message);
                        break;

                    case SipMethod.OPTIONS:
                        HandleOptions(message);
                        break;

                    case SipMethod.ACK:
                        HandleAck(message);
                        break;

                    case SipMethod.BYE:
                        HandleBye(message);
                        break;

                    case SipMethod.CANCEL:
                        HandleCancel(message);
                        break;

                    case SipMethod.REGISTER:
                        HandleRegister(message);
                        break;

                    case SipMethod.NOTIFY:
                        HandleNotify(message);
                        break;

                    case SipMethod.INFO:
                        HandleInfo(message);
                        break;

                    case SipMethod.UPDATE:
                        HandleUpdate(message);
                        break;

                    case SipMethod.MESSAGE:
                        HandleMessage(message);
                        break;

                    case SipMethod.SUBSCRIBE:
                        throw new NotImplementedException("SipMethod.SUBSCRIBE");

                    case SipMethod.COMET:
                        throw new NotImplementedException("SipMethod.COMET");

                    case SipMethod.PRACK:
                        throw new NotImplementedException("SipMethod.PRACK");
                }
            }
            catch (DialogNotFoundException e)
            {
                Logger.WriteInfo(e.Message);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Exception on handling request CallID:{0}", message.CallID);
            }
        }

        void HandleSipResponse(SipMessage message)
        {
            if (message.HeaderFirstLine.ResponseHeader.StatusCode == StatusCode.DoNotDisturb/*(StatusCode)Config.Default.StatusCodeOnDndRejection*/)
            {
                SendAck(message);
                HandleDnd(message);
                return;
            }

            switch (message.HeaderFirstLine.ResponseHeader.StatusCode)
            {
                case StatusCode.Ringing:
                    HandleRinging(message);
                    break;

                case StatusCode.Ok:
                    HandleOk(message);
                    break;

                case StatusCode.NotFound:
                    HandleNotFound(message);
                    break;

                case StatusCode.Busy_Here:
                case StatusCode.Decline:
                    HandleBusyHere(message);
                    break;

                case StatusCode.ServerError:
                    HandleServerError(message);
                    break;

                case StatusCode.RequestTerminated:
                    HandleRequestTerminated(message);
                    break;

                case StatusCode.Accept:
                    HandleAccept(message);
                    break;

                case StatusCode.BadRequest:
                    HandleBadRequest(message);
                    break;

                case StatusCode.BusyEveryWhere:
                    HandleBusyEveryWhere(message);
                    break;

                case StatusCode.TransactionDoesNotExist:
                    HandleTransactionDoesNotExist(message);
                    break;

                case StatusCode.TemporarilyUnavailable:
                    HandleTemporarilyUnavailable(message);
                    break;

                case StatusCode.Unauthorized:
                    HandleUnAuthorized(message);
                    break;

                case StatusCode.SessionProgress:
                    HandleSessionProgress(message);
                    break;

                case StatusCode.ServiceUnavailable:
                    HandleServiceUnavailable(message);
                    break;

                case StatusCode.Trying:
                    HandleTrying(message);
                    break;

                case StatusCode.UnsupportedMediaType:
                    HandleUnsupportedMediaType(message);
                    break;

                case StatusCode.Forbidden:
                    Logger.WriteWarning("Forbidden: {0}", message.CallID);
                    break;

                case StatusCode.Unknown_1:
                    Logger.WriteWarning("Unknown status: {0}", (int)StatusCode.Unknown_1);
                    break;

                default:
                    SendAck(message);
                    Logger.WriteWarning("Status Code: {0}", message.HeaderFirstLine.ResponseHeader.StatusCode);
                    throw new NotImplementedException(message.HeaderFirstLine.ResponseHeader.StatusCode.ToString());
            }
        }

        private void HandleDnd(SipMessage message)
        {
            var dialog = FindDialog(message);
            if (dialog.DialogType == DialogType.ClientIncomming) // forward call from gateway to client which is DND
            {
                Logger.WriteDebug("DND detected on operator: {0}, dialog:{1}", dialog.ToAccount.UserID, dialog.DialogID);
                dialog.ToAccount.Status = SipAccountStatus.DND;

                if (dialog.DivertPartner != null && dialog.TransferFailureTime < Constants.TransferFailureMaxTime)
                {
                    Logger.WriteWarning("Diverting for dialog {0} failed because of DND, master dialog: {1}, dialog.DivertPartner.Status: {2}", dialog.DialogID, dialog.DivertPartner.DialogID, dialog.DivertPartner.Status);
                    dialog.Call.DisconnectCause = Config.Default.DisconnectCauseOnDndRejection;
                    dialog.ToAccount.LastCallTime = DateTime.Now; // to select another operator, or wait some seconds to divert again
                    dialog.TransferFailureTime++;
                    TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                    dialog.Disconnected((DisconnectCause)Config.Default.DisconnectCauseOnDndRejection);
                }
                else
                    HandleCallDisconnected(dialog, (DisconnectCause)Config.Default.DisconnectCauseOnDndRejection);
            }
            else
                Logger.WriteException("HandleDnd on status: {0}", message.HeaderFirstLine.ResponseHeader.StatusCode);
        }

        private void HandleMessage(SipMessage message)
        {
            var user = Accounts.FirstOrDefault(a => a.UserID == message.HeaderFirstLine.RequestHeader.MethodUri.UserID);
            message.ChangeAsResponse(StatusCode.Ok);
            sipNet.Send(message.Content, message.Sender);

            if (message.SdpFields.Any(f => f.Content.StartsWith("s=DND")))
            {
                var field = message.SdpFields.First(f => f.Content.StartsWith("s=DND"));
                if (user != null)
                {
                    if (field.Value == "DND:on")
                    {
                        user.Status = SipAccountStatus.DND;
                        Logger.WriteDebug("User {0} DND start.", user.UserID);
                    }
                    else
                    {
                        user.Status = SipAccountStatus.Idle;
                        Logger.WriteDebug("User {0} DND end.", user.UserID);
                    }
                }
            }
        }

        private void HandleUpdate(SipMessage message)
        {
            Logger.WriteWarning("UPDATE message not handled.");
        }

        private void HandleInfo(SipMessage message)
        {
            if (message.ContentType.Value != null && message.ContentType.Value.ToLower().Contains("dtmf-relay"))
            {
                if (message.SdpFields.Any(f => f.FieldName.ToLower() == "signal"))
                {
                    var field = message.SdpFields.First(f => f.FieldName.ToLower() == "signal");
                    var dialog = FindDialog(message);

                    Logger.WriteDebug("DTMF detected: {0}", field.Value);
                    message.ChangeAsResponse(StatusCode.Ok);
                    sipNet.Send(message.Content, message.Sender);

                    if (DtmfDetected != null)
                        DtmfDetected(this, new DtmfDetectedEventArgs(dialog)
                        {
                            Key = field.Value.First()
                        });
                    return;
                }
            }

            throw new NotImplementedException("SipMethod.INFO");
        }

        void HandleBadRequest(SipMessage message)
        {
            var dialog = FindDialog(message);
            message.ChangeAsRequest(SipMethod.ACK);
            message.HeaderFirstLine.RequestHeader.MethodUri = dialog.InviteMessage.HeaderFirstLine.RequestHeader.MethodUri;
            sipNet.Send(message.Content, message.Sender);
        }

        void HandleRefer(SipMessage message)
        {
            var dialog = FindDialog(message);
            if (dialog.FromAccount != null && dialog.ToAccount != null) // A ---INVITE---> B ---REFER---> C
            {
                message.RemoveField(message.Route);
                dialog.ForwardReferViaBranch = GenerateBranch();
                message.PushVia(dialog.ForwardReferViaBranch);
                message.AppendRecordRoute();
                ForwardMessage(message, dialog);
            }
            else // A ---INVITE---> IVR ---INVITE---> B ---REFER---> TARGET
            {
                HandleReferOnDialogInProxyTransferMode(message, dialog);
            }
        }

        void HandleReferOnDialogInProxyTransferMode(SipMessage message, SipDialog dialog)
        {
            SipMessage msg = (SipMessage)message.Clone();
            msg.ChangeAsResponse(StatusCode.Accept);
            sipNet.Send(msg.Content, dialog.ToAccount.SipEndPoint);

            var account = Accounts.FirstOrDefault(p => p.UserID == message.ReferTo.Uri.UserID);
            if (account != null) // B ---REFER---> C
            {
                var attendedDialog = Dialogs.FirstOrDefault(d => d.FromAccount == dialog.ToAccount && d.ToAccount == account);

                if (attendedDialog != null // Attended Transfer (first creates an invite and must be replaced
                    || (account.Status == SipAccountStatus.Idle && account.MaxConcurrentCalls == 1))
                {
                    dialog.Status = DialogStatus.Refering;
                    SendNotify(message.Sender, message.ReferredBy.Uri.UserID, message.Sender,
                        dialog.DialogID, dialog.InviteMessage.From.Tag, message.From.Tag, 2, StatusCode.Trying);

                    string replaces = null;
                    if (attendedDialog != null)
                        replaces = string.Format("{0};to-tag={1};from-tag={2}", attendedDialog.DialogID, attendedDialog.InviteMessage.To.Tag, attendedDialog.InviteMessage.From.Tag);

                    SipDialog targetDialog = Dial(dialog.CallerID, message.ReferTo.Uri.UserID, account, message.ReferredBy.Uri.UserID, dialog.Call.GraphTrack, replaces);

                    targetDialog.DivertPartner = dialog.DivertPartner;
                    targetDialog.ReferMessage = message;

                    dialog.ReferMessage = message;

                    Logger.WriteView("HandleReferOnDialogInProxyTransferMode, dialog:{0}", dialog.DialogID);
                }
                else
                {
                    SendNotify(message.Sender, message.ReferredBy.Uri.UserID, message.Sender,
                        dialog.DialogID, dialog.InviteMessage.From.Tag, message.From.Tag, 4, StatusCode.Busy_Here);
                }
            }
            else // B ---REFER---> IVR
            {
                if (HasGraph(message.ReferTo.Uri.UserID)) // Refer to IVR
                {
                    SendNotify(message.Sender, message.ReferredBy.Uri.UserID, message.Sender,
                        dialog.DialogID, dialog.InviteMessage.From.Tag, message.From.Tag, 2, StatusCode.Ok);

                    // TODO: complete this
                }
                else // 3- No Graph and No Account
                {
                    SendNotify(message.Sender, message.ReferredBy.Uri.UserID, message.Sender,
                        dialog.DialogID, dialog.InviteMessage.From.Tag, message.From.Tag, 4, StatusCode.Busy_Here);
                }
            }
        }

        void HandleRegister(SipMessage message)
        {
            if (Config.Default.OnlyAcceptEmbeddedSoftPhoneRegistration)
                if (!message.UserAgent.IsAgentValid)
                {
                    Logger.WriteWarning("Registration dropped due to lack of permission. user: {0}", message.From.Uri.UserID);
                    return;
                }

            var account = Accounts.FirstOrDefault(a => a.UserID == message.From.Uri.UserID);

            bool isUnregister = CheckForUnregister(message, account);
            if (!isUnregister)
            {
                if (account == null)
                {
                    if (Config.Default.AllowRegisterAnonymousAccount)
                    {
                        account = new SipAccount()
                        {
                            UserID = message.From.Uri.UserID,
                            Contact = message.Contact.Value,
                        };

                        using (FolderDataContext dc = new FolderDataContext())
                        {
                            account.FolderUser = dc.Users.FirstOrDefault(u => u.Username == account.UserID);
                        }
                        Accounts.Add(account);
                    }
                    else
                    {
                        using (FolderDataContext context = new FolderDataContext())
                        {
                            User user = context.Users.Where(t => t.Username == message.From.Uri.UserID && t.IsEnable).SingleOrDefault();
                            if (user == null)
                            {
                                Logger.WriteWarning("Reject Register for not existed user :{0}", message.From.Uri.UserID);
                                return;
                            }
                            else
                            {
                                //if (user[Constants.UserProfileKey_VoipPassword] == message.From.Uri.UserID)
                                //{ 
                                account = new SipAccount()
                                {
                                    UserID = message.From.Uri.UserID,
                                    Contact = message.Contact.Value,
                                };

                                using (FolderDataContext dc = new FolderDataContext())
                                {
                                    account.FolderUser = dc.Users.FirstOrDefault(u => u.Username == account.UserID);
                                }
                                Accounts.Add(account);
                                //}
                            }

                        }
                    }
                }
                else if (
                    (Config.Default.ClientLoginMode == ClientLoginMode.SingleAddress && !IsSame(account.SipEndPoint, message.Via.EndPoint)) ||
                    (Config.Default.ClientLoginMode == ClientLoginMode.SingleMachine && !IsSame(account.SipEndPoint.Address, message.Via.EndPoint.Address))
                    )
                {
                    Logger.WriteDebug("Account '{0}' client address changed from '{1}' to '{2}'", account.UserID, account.SipEndPoint, message.Via.EndPoint);
                    if (!account.ClientAddressChangeTime.HasValue || account.ClientAddressChangeTime < DateTime.Now.AddMinutes(-5))
                    {
                        account.ClientAddressChangeTime = DateTime.Now;
                    }
                    else
                    {
                        Logger.WriteWarning("Account '{0}' client address could not changed from '{1}' to '{2}', send register failure.", account.UserID, account.SipEndPoint, message.Via.EndPoint);
                        message.ChangeAsResponse(StatusCode.Forbidden);
                        sipNet.Send(message.Content, message.Via.EndPoint);

                        account.ClientAddressChangeTime = null; // In case second time for login
                        return;
                    }
                }
                else if (account != null)
                {
                    if (!Config.Default.AllowRegisterAnonymousAccount)
                    {
                        using (FolderDataContext context = new FolderDataContext())
                        {
                            User user = context.Users.Where(t => t.Username == account.UserID && t.IsEnable).SingleOrDefault();
                            if (user == null)
                            {
                                Logger.WriteWarning("Reject Register for not existed user :{0}, account is not null", account.UserID);
                                return;
                            }
                        }
                    }
                }

                account.SipEndPoint = message.Via.EndPoint;
                account.RegisterTime = DateTime.Now;

                if (message.Expires != null)
                    account.ExpireTime = DateTime.Now.AddSeconds(message.Expires.Seconds);
                else
                    account.ExpireTime = DateTime.Now.AddSeconds(Config.Default.DefaultExpireSeconds);

                SessionManager.OnAccountRegister(account);

                // Sends ok before any message raising
                message.ChangeAsResponse(StatusCode.Ok);
                sipNet.Send(message.Content, message.Via.EndPoint);

                if (account.Status == SipAccountStatus.Offline)
                {
                    account.Status = SipAccountStatus.Idle;
                    Logger.WriteInfo("Account {0} registered.", message.From.Uri.UserID);
                    if (Registered != null)
                        Registered(this, new RegisteredEventArgs(account));
                }
                //else
                //    Logger.WriteView("Account {0} registered, status: {1}", message.From.Uri.UserID, account.Status);
            }
            else
            {
                message.ChangeAsResponse(StatusCode.Ok);
                sipNet.Send(message.Content, message.Via.EndPoint);
            }
        }

        void HandleRequestTerminated(SipMessage message)
        {
            var dialog = FindDialog(message);

            switch (dialog.Status)
            {
                case DialogStatus.Disconnected:
                case DialogStatus.CancelingWaitForAck:
                    SendTemplateMessage(Template_Ack, dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value);
                    HandleCallDisconnected(dialog, DisconnectCause.NormalUnspecified);
                    break;

                default:
                    throw new NotImplementedException("RequestTerminated on forwardmode, status: " + dialog.Status.ToString());
            }
        }

        void HandleServerError(SipMessage message)
        {
            HandleFailure(message, DisconnectCause.UnknownProblem);
        }

        void HandleNotify(SipMessage message)
        {
            var dialog = FindDialog(message);
            //if (dialog == null)
            //{
            //    message.ChangeAsResponse(StatusCode.Ok);
            //    sipNet.Send(message.Content, message.Sender);
            //    return;
            //}

            if (dialog.IsForwardMode)
            {
                dialog.Status = DialogStatus.Refering;
                message.RemoveField(message.Route);
                message.PushVia(GenerateBranch());
                message.AppendRecordRoute();
                ForwardMessage(message, dialog);
            }
            else
            {
                message.ChangeAsResponse(StatusCode.Ok);
                sipNet.Send(message.Content, message.Via.EndPoint);

                switch (message.NotifyStatusCode.Value)
                {
                    case StatusCode.Ok:
                        HandleCallDisconnected(dialog, DisconnectCause.NormalCallClearing);
                        break;

                    case StatusCode.Ringing:
                        OnTargetRinging(this, new TargetRingingEventArgs(dialog));
                        break;

                    case StatusCode.ServiceUnavailable:
                    case StatusCode.NotFound:
                    case StatusCode.Decline:
                    case StatusCode.RequestTerminated:
                    case StatusCode.Busy_Here:
                        HandleCallDisconnected(dialog, DisconnectCause.UserBusy);
                        //OnCallDisconnected(this, new CallDisconnectedEventArgs(dialog));
                        break;

                    //case StatusCode.Busy_Here:
                    //CallDisconnected(dialog, DisconnectCause.UserBusy);
                    ////dialog.DisconnectCause = DisconnectCause.UserBusy;
                    //OnCallDisconnected(this, new CallDisconnectedEventArgs(dialog));
                    //break;
                }
            }
        }

        void HandleAccept(SipMessage message)
        {
            var dialog = FindDialog(message);

            if (dialog.IsForwardMode)
            {
                message.PopVia(dialog.ForwardReferViaBranch);
                ForwardMessage(message, dialog);
            }
            else
            {
                if (dialog.Status != DialogStatus.ReferingWaitForAccept)  // && dialog.Status != DialogStatus.DivertingWaitForTargetResponse, dial from IPPhone to IVR, forward to mobile
                    throw new InvalidOperationException("REFER Accept on status " + dialog.Status.ToString() + ", dialog: " + dialog.DialogID);
            }

            Logger.WriteDebug("Refer Accepted, callID :{0}", dialog.DialogID);
        }

        void HandleRinging(SipMessage message)
        {
            var dialog = FindDialog(message);

            if (dialog.DivertPartner != null && dialog.DivertPartner.Status == DialogStatus.DivertingWaitForTargetResponse)
            {
                dialog.DivertPartner.Status = DialogStatus.WaitForDiverting;
                OnTargetRinging(this, new TargetRingingEventArgs(dialog.DivertPartner)); // check this
            }

            switch (dialog.Status)
            {
                case DialogStatus.DialingWaitForOk:
                case DialogStatus.Dialing:
                case DialogStatus.Invite:

                    dialog.Status = DialogStatus.Ringing;
                    dialog.InviteMessage.To.Tag = message.To.Tag;
                    break;

                case DialogStatus.Ringing:
                    return;

                case DialogStatus.Disconnected:
                case DialogStatus.CancelingWaitForAck:
                case DialogStatus.ByingWaitForOk:
                    Logger.WriteWarning("Ringing on status:" + dialog.Status);
                    return;

                default:
                    throw new NotImplementedException("Ringing on status:" + dialog.Status);
            }

            if (dialog.IsForwardMode)
            {
                dialog.RingingTo = message.To;
                message.PopVia(dialog.ForwardInviteViaBranch);
                ForwardMessage(message, dialog);
            }
        }

        void HandleCancel(SipMessage message)
        {
            var dialog = FindDialog(message);

            //if (dialog != null && dialog.IsForwardMode)
            if (dialog.IsForwardMode)
            {
                SipMessage cancelMessage = dialog.InviteMessage.Clone() as SipMessage;
                cancelMessage.ClearSdp();
                cancelMessage.HeaderFields.Remove(cancelMessage.ContentType);
                cancelMessage.HeaderFirstLine.RequestHeader.Method = SipMethod.CANCEL;
                cancelMessage.CSeq.Method = SipMethod.CANCEL;
                sipNet.Send(cancelMessage.Content, dialog.ToAccount.SipEndPoint);
            }

            message.ChangeAsResponse(StatusCode.Ok);
            sipNet.Send(message.Content, message.Via.EndPoint);

            message.ChangeAsResponse(StatusCode.RequestTerminated);
            sipNet.Send(message.Content, message.Via.EndPoint);

            //if (dialog != null)
            dialog.Status = DialogStatus.CancelingWaitForAck;
        }

        void HandleBye(SipMessage byeMessage)
        {
            var dialog = FindDialog(byeMessage);

            DisconnectCause disconnectCause;
            if (IsSame(byeMessage.Sender, dialog.InviteMessage.Sender))
            {
                disconnectCause = DisconnectCause.NormalUnspecified;
            }
            else
            {
                disconnectCause = DisconnectCause.NormalCallClearing;
            }

            if (dialog.IsForwardMode)
            {
                ForwardMessage(byeMessage, dialog);
                dialog.Status = DialogStatus.ByingWaitForOk;
                HandleCallDisconnected(dialog, disconnectCause);
            }
            else
            {
                byeMessage.ChangeAsResponse(StatusCode.Ok);
                sipNet.Send(byeMessage.Content, byeMessage.Via.EndPoint);

                // FIXME: I think DisconnectedDialogsRecordedVoices is useless, if you find its usage tell me !!!
                if (dialog.Status == DialogStatus.Recording)
                    lock (DisconnectedDialogsRecordedVoices)
                    {
                        DisconnectedDialogsRecordedVoices[dialog.DialogID] = dialog.RecordedVoiceStream.ToArray();
                    }
                //////////////////////

                HandleCallDisconnected(dialog, disconnectCause);
            }

            if ((dialog.AuditionTarget != null) || (dialog.DivertPartner != null && dialog.DivertPartner.AuditionTarget != null))
                DisconnectEavesdropperDialog(dialog);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DisconnectEavesdropperDialog(SipDialog dialog)
        {
            try
            {
                if (dialog == null)
                    return;

                if (dialog.AuditionTarget != null)
                {
                    SipDialog auditDialog = FindDialog(dialog.AuditionTarget.DialogID);
                    if (auditDialog == null)
                        return;

                    auditDialog.Status = DialogStatus.ByingWaitForOk;
                    auditDialog.ByeTarget = auditDialog.Partner.SipEndPoint;
                    auditDialog.ByeMessage = SendTemplateMessage(Template_Bye, auditDialog.Partner.SipEndPoint, auditDialog.DialogID, auditDialog.InviteMessage.To.Value, auditDialog.InviteMessage.From.Value, auditDialog.InviteMessage.From.Tag, auditDialog.Partner.UserID);
                }

                if (dialog.DivertPartner != null && dialog.DivertPartner.AuditionTarget != null)
                {
                    SipDialog auditDialog = FindDialog(dialog.DivertPartner.AuditionTarget.DialogID);
                    if (auditDialog == null)
                        return;

                    auditDialog.Status = DialogStatus.ByingWaitForOk;
                    auditDialog.ByeTarget = auditDialog.Partner.SipEndPoint;
                    auditDialog.ByeMessage = SendTemplateMessage(Template_Bye, auditDialog.Partner.SipEndPoint, auditDialog.DialogID, auditDialog.InviteMessage.To.Value, auditDialog.InviteMessage.From.Value, auditDialog.InviteMessage.From.Tag, auditDialog.Partner.UserID);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                Logger.WriteError("An Error occured while disconnecting eavesdropper dialog. target dialog:{0} | divert partner:{1}", dialog.DialogID, dialog.DivertPartner == null ? "no parter" : dialog.DivertPartner.DialogID);
            }
        }

        void HandleOptions(SipMessage message)
        {
            message.ChangeAsResponse(StatusCode.Ok);
            sipNet.Send(message.Content, message.Via.EndPoint);
        }

        void HandleOk(SipMessage message)
        {
            if (message.CSeq.Method == SipMethod.REGISTER)
            {
                var account = FindFromAccount(message.From.Uri.UserID, message.Sender.Address);

                if (message.Contact.Expires == "0")
                {
                    if (UnRegistered != null)
                        UnRegistered(this, new UnRegisteredEventArgs() { Account = account });

                    Logger.WriteInfo("UnRegistration OK!");
                }
                else
                {
                    if (Registered != null)
                        Registered(this, new RegisteredEventArgs(account));

                    Logger.WriteInfo("Registration OK!");
                }
                return;
            }


            if (message.CSeq.Method == SipMethod.NOTIFY)
            {
            }


            var dialog = FindDialog(message);
            switch (dialog.Status)
            {
                //case DialogStatus.InviteWaitForAck:
                //    if (dialog.IsForwardMode)
                //    {
                //        dialog.Call.AnswerTime = DateTime.Now;

                //        ///*
                //        // * I think the way that Cisco interprets Ack message depends on it's iOS version.
                //        // * For instance, Ack headers like 1113@x.y.z.t:5060 (which 1113 is the Cisco's account) for outcalls are accepted in 
                //        // * one router while they are droped in another!
                //        // * Both routers have the same configuration but different iOS; first one, iOS 12.4 (9) and second one, iOS 12.4 (5a).
                //        // * Finally, based on RFC 3261 we changed the header of Ack message for outcalls in order to make it understandable for
                //        // * all iOS versions.
                //        // * For more information refer to RFC 3261, page 129, Construction of the ACK Request.
                //        // */
                //        if (dialog.DialogType == DialogType.ClientOutgoing || dialog.DialogType == DialogType.GatewayOutgoing)
                //            SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.Extension, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                //        else
                //            SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                //    }
                //    break;

                case DialogStatus.Dialing:
                case DialogStatus.DialingWaitForOk:
                case DialogStatus.Ringing:
                case DialogStatus.Invite:
                    if (message.CSeq.Method != SipMethod.INVITE)
                        throw new NotImplementedException("HandleOK on CSEQ: " + message.CSeq.Method + ", status:" + dialog.Status);

                    dialog.Status = DialogStatus.Connect;
                    ChangeAccountStatusToTalking(dialog);

                    if (dialog.IsForwardMode)
                    {
                        message.PopVia(dialog.ForwardInviteViaBranch);
                        dialog.Status = DialogStatus.InviteWaitForAck;
                        ForwardMessage(message, dialog);
                        //sipNet.Send(message.Content, dialog.FromAccount.SipEndPoint);
                    }
                    else
                    {
                        dialog.Call.AnswerTime = DateTime.Now;

                        ///*
                        // * I think the way that Cisco interprets Ack message depends on it's iOS version.
                        // * For instance, Ack headers like 1113@x.y.z.t:5060 (which 1113 is the Cisco's account) for outcalls are accepted in 
                        // * one router while they are droped in another!
                        // * Both routers have the same configuration but different iOS; first one, iOS 12.4 (9) and second one, iOS 12.4 (5a).
                        // * Finally, based on RFC 3261 we changed the header of Ack message for outcalls in order to make it understandable for
                        // * all iOS versions.
                        // * For more information refer to RFC 3261, page 129, Construction of the ACK Request.
                        // */
                        if ((dialog.DialogType == DialogType.ClientOutgoing || dialog.DialogType == DialogType.GatewayOutgoing) && !string.IsNullOrWhiteSpace(dialog.Extension))
                            SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.Extension, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);
                        else
                            SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);

                        dialog.RtpNet.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(message.RtpAddress), message.RtpPort);
                        dialog.RtpNet.OnReceive += new PacketTransmitEventHandler(RtpNet_OnReceive);
                        dialog.RtpNet.Start();
                        Logger.WriteInfo("Receive RTP from '{0}' started on OK message, dialog:'{1}'", dialog.RtpNet.RemoteEndPoint, dialog.DialogID);

                        if (dialog.DivertPartner != null)
                        {
                            Logger.WriteDebug("RTP redirects to '{0}', dialog: {1}, partner: {2}", dialog.DivertPartner.RtpNet.RemoteEndPoint, dialog.DialogID, dialog.DivertPartner.DialogID);
                            dialog.RecordedVoiceStream = new System.IO.MemoryStream();
                            dialog.DivertPartner.RecordedVoiceStream = new System.IO.MemoryStream();
                            dialog.Status = DialogStatus.Talking;
                            dialog.DivertPartner.Status = DialogStatus.Talking;
                            dialog.DivertPartner.DivertCallID = dialog.Call.DialogID;
                            if (dialog.DialogType == DialogType.ClientOutgoing || dialog.DialogType == DialogType.ClientIncomming)
                                dialog.DivertPartner.AgentID = dialog.DialogType == DialogType.ClientIncomming ? dialog.ToAccount.UserID : dialog.FromAccount.UserID;
                            //dialog.DivertPartner.DivertTime = DateTime.Now;
                            //dialog.DivertTime = DateTime.Now;

                            if (dialog.DivertPartner.DivertPartner != dialog) // A->Proxy->B-Refer->C: Ok(C) on Refering mode
                            {
                                var sourceDialog = dialog.DivertPartner.DivertPartner;
                                sourceDialog.DivertPartner = null;

                                SendNotify(sourceDialog.ReferMessage.Sender, sourceDialog.ReferMessage.ReferredBy.Uri.UserID,
                                    sourceDialog.ReferMessage.Sender, sourceDialog.DialogID, sourceDialog.InviteMessage.From.Tag,
                                    sourceDialog.ReferMessage.From.Tag, 4, StatusCode.Ok);

                                dialog.DivertPartner.DivertPartner = dialog;
                                break;
                            }
                        }


                        //if (dialog.CallerID.StartsWith("UMS"))
                        if (dialog.IsInforming)
                        {
                            if (ScheduledOutcallStablished != null)
                                ScheduledOutcallStablished(this, new CallStablishedEventArgs(dialog));
                        }
                        else
                            CallStablished(this, new CallStablishedEventArgs(dialog));
                    }
                    break;

                case DialogStatus.Hold:
                    if (message.CSeq.Method != SipMethod.INVITE)
                        throw new NotImplementedException("HandleOK on Hold mode CSEQ: " + message.CSeq.Method + ", status:" + dialog.Status);

                    if (dialog.IsForwardMode)
                    {
                        message.PopVia(dialog.ForwardInviteViaBranch);
                        ForwardMessage(message, dialog);
                    }
                    else
                    {
                        message.ChangeAsRequest(SipMethod.ACK);
                        sipNet.Send(message.Content, message.Sender);
                    }
                    break;

                //case DialogStatus.UnHoldingWaitForOk:
                //    if (dialog.IsForwardMode)
                //    {
                //        message.PopVia(dialog.ForwardInviteViaBranch);
                //        ForwardMessage(message, dialog);
                //        dialog.Status = DialogStatus.UnHoldingWaitForAck;
                //    }
                //    break;

                case DialogStatus.CancelingWaitForAck:
                    // Do nothing, wait for 487
                    break;

                case DialogStatus.Disconnected:
                case DialogStatus.ByingWaitForOk:
                case DialogStatus.DivertingWaitForTargetResponse:
                    if (message.CSeq.Method == SipMethod.INVITE)
                    {
                        Logger.WriteWarning("OK on CSEQ INVITE when status is ByingWaitForOk/Disconnected, callID:{0}", dialog.Call.DialogID);
                        break;
                    }

                    if (message.CSeq.Method != SipMethod.BYE && message.CSeq.Method != SipMethod.NOTIFY && message.CSeq.Method != SipMethod.CANCEL)
                        throw new NotImplementedException("OK on CSEQ: " + message.CSeq.Method + ", status:" + dialog.Status + ", callID: " + dialog.Call.DialogID);

                    if (dialog.IsForwardMode)
                        ForwardMessage(message, dialog);
                    //else
                    //{
                    //    dialog.SaveCall(DisconnectCause.NormalCallClearing);
                    //    OnCallDisconnected(this, new CallDisconnectedEventArgs(dialog));
                    //}

                    if (message.CSeq.Method == SipMethod.BYE)
                        HandleCallDisconnected(dialog, DisconnectCause.NormalCallClearing);
                    break;

                case DialogStatus.Refering:
                    if (dialog.FromAccount != null && dialog.ToAccount != null)
                        ForwardMessage(message, dialog);
                    break;

                case DialogStatus.ReInvitingForFax:
                    Logger.WriteTodo("Check this: OK on status: {0}, dialog:{1} callerid:{2}, localport:{3}", dialog.Status, dialog.DialogID, dialog.CallerID, dialog.RtpNet.LocalPort);
                    dialog.MediaType = SdpFieldMedia.MediaType.Image;
                    SendAck(message);
                    break;

                case DialogStatus.Connect:
                case DialogStatus.Talking:
                    Logger.WriteDebug("OK on status: {0}, dialog:{1} callerid:{2}", dialog.Status, dialog.DialogID, dialog.CallerID);
                    SendAck(dialog.ToAccount.SipEndPoint, dialog.CallerID, dialog.CalleeID, message.CallID, message.Via.Branch, message.From.Tag, message.To.Value, dialog.Extension, dialog.DialogType);

                    if (dialog.DivertPartner != null) // In case session progress already come, start redirecting voices
                    {
                        Logger.WriteDebug("RTP redirects to '{0}', dialog: {1}, partner: {2}", dialog.DivertPartner.RtpNet.RemoteEndPoint, dialog.DialogID, dialog.DivertPartner.DialogID);
                        dialog.RecordedVoiceStream = new MemoryStream();
                        dialog.DivertPartner.RecordedVoiceStream = new MemoryStream();
                    }

                    break;

                default:
                    Logger.WriteWarning("OK on CSEQ:{0}, status:{1}", message.CSeq.Method, dialog.Status);
                    //throw new NotImplementedException("OK on CSEQ: " + message.CSeq.Method + ", status:" + dialog.Status);
                    break;
            }
        }

        void HandleAck(SipMessage ackMessage)
        {
            var dialog = FindDialog(ackMessage);
            Logger.WriteDebug("Dialog status on ack is: {0}", dialog.Status);
            switch (dialog.Status)
            {
                case DialogStatus.InviteWaitForAck:
                    if (dialog.IsForwardMode)
                    {
                        ackMessage.PushVia("0");
                        ackMessage.RemoveField(ackMessage.Route);
                        ackMessage.AppendRecordRoute();
                        sipNet.Send(ackMessage.Content, dialog.ToAccount.SipEndPoint);
                        dialog.Status = DialogStatus.Connect;
                        dialog.Call.AnswerTime = DateTime.Now;
                    }
                    else
                    {
                        dialog.PassThroughFaxStream = new MemoryStream();
                        dialog.RtpNet.OnReceive += new PacketTransmitEventHandler(RtpNet_OnReceive);
                        dialog.RtpNet.Start();

                        Logger.WriteInfo("Receive RTP from '{0}' started on ACK message, dialog:'{1}'", dialog.RtpNet.RemoteEndPoint, dialog.DialogID);
                        dialog.Status = DialogStatus.Connect;
                        dialog.Call.AnswerTime = DateTime.Now;
                        if (CallStablished != null)
                            CallStablished(this, new CallStablishedEventArgs(dialog));

                        //if (dialog.ToAccount != null)
                        //    dialog.ToAccount.TemporarilyUnavailableCount = 0;
                    }

                    //if (dialog.InviteMessage.ReferredBy != null)
                    //{
                    //    var refferingDialog = Dialogs.FirstOrDefault(d => d.Status == DialogStatus.WaitForRefferringDialogAck &&
                    //        (d.Partner.SipEndPoint.ToString() == dialog.Partner.SipEndPoint.ToString()));

                    //    if (refferingDialog == null)
                    //        throw new NotImplementedException("refferingDialog not found, callid: " + ackMessage.CallID);

                    //    DisconnectDialog(refferingDialog);
                    //}

                    break;

                case DialogStatus.FaxOKWaitForAck:

                    //pass-through fax mode
                    if (dialog.MediaType == SdpFieldMedia.MediaType.Audio)
                    {
                        Logger.WriteInfo("Receive FAX RTP in pass-through mode from '{0}' started on ACK message, dialog:'{1}'", dialog.RtpNet.RemoteEndPoint, dialog.DialogID);

                        dialog.Status = DialogStatus.Connect;
                        dialog.Call.AnswerTime = DateTime.Now;

                        if (FaxStablished != null)
                            FaxStablished(this, new CallStablishedEventArgs(dialog));
                    }

                    break;

                case DialogStatus.RejectWaitForAck:
                    HandleCallDisconnected(dialog, DisconnectCause.CallRejected);
                    //ReleaseDialog(dialog);
                    break;

                case DialogStatus.CancelingWaitForAck:
                case DialogStatus.ByingWaitForOk:
                    // Do nothing
                    break;

                case DialogStatus.UserBusySentWaitForAck:
                    HandleCallDisconnected(dialog, DisconnectCause.UserBusy);
                    //ReleaseDialog(dialog);
                    break;

                case DialogStatus.Talking:
                case DialogStatus.Hold:
                    if (dialog.IsForwardMode)
                        ForwardMessage(ackMessage, dialog);
                    break;

                case DialogStatus.Invite: // if we send ProxyAuthenticationRequired on invite
                    HandleCallDisconnected(dialog, DisconnectCause.SourceIsNotRegistered);
                    //dialog.SaveCall(DisconnectCause.SourceIsNotRegistered);
                    //ReleaseDialog(dialog);
                    break;

                case DialogStatus.ReInvitingForFax:
                case DialogStatus.Disconnected:
                    Logger.WriteDebug("ACK on call in status {0}", dialog.Status);
                    break;

                default:
                    Logger.WriteWarning("ACK on call in status {0}", dialog.Status);
                    //throw new NotSupportedException("ACK on call in status " + dialog.Status.ToString());
                    break;
            }
        }

        void HandleInviteInSoftPhoneMode(SipDialog dialog)
        {
            if (IncommingCall != null)
                IncommingCall(this, new IncommingCallEventArgs(dialog));
        }

        void HandleInvite(SipMessage message)
        {
            if (message.ContentLength.Length == 0)
            {
                Logger.WriteWarning("Empty length Invite: {0}", message.CallID);
                return;
            }

            if (CheckAndHandleInviteForPassThroughFax(message))
                return;

            if (CheckAndHandleInviteForFax(message))
                return;

            if (CheckAndHandleInviteForHold(message))
            {
                Logger.WriteInfo("INVITE on already existed dialog, {0}", message.CallID);
                return;
            }

            // New Dialog
            SipDialog dialog = NewDialog(DialogStatus.Invite, message.CallID, message.From.Uri.UserID, message.To.Uri.UserID);
            dialog.InviteMessage = message;
            dialog.RtpNet.RemoteEndPoint = message.RtpEndPoint;

            if (IsSoftPhoneMode)
            {
                HandleInviteInSoftPhoneMode(dialog);
                Dialogs.Remove(dialog);
                return;
            }

            dialog.FromAccount = FindFromAccount(message.From.Uri.UserID, message.Sender.Address);

            if (dialog.FromAccount == null)
            {
                Logger.WriteWarning("From Account '{0}' not found or not registered.", message.From.Uri.UserID);
                message.ChangeAsResponse(StatusCode.ProxyAuthenticationRequired);
                Dialogs.Remove(dialog);
                sipNet.Send(message.Content, message.Via.EndPoint);
                return;
            }

            dialog.DialogType = dialog.FromAccount.MaxConcurrentCalls == 1 ? DialogType.ClientOutgoing : DialogType.GatewayIncomming;

            #region Check for privileged dial targets
            if (dialog.FromAccount.FolderUser != null && !string.IsNullOrWhiteSpace(dialog.FromAccount.FolderUser[Constants.UserProfileKey_PrivilegedDialTargets]))
            {
                if (!Regex.IsMatch(message.To.Uri.UserID, dialog.FromAccount.FolderUser[Constants.UserProfileKey_PrivilegedDialTargets]))
                {
                    Logger.WriteWarning("Dial permission failed, for account '{0}' dialing '{1}'. Privileged Dial Targets: {2}", dialog.FromAccount.UserID, message.To.Uri.UserID, dialog.FromAccount.FolderUser[Constants.UserProfileKey_PrivilegedDialTargets]);
                    dialog.Disconnected(DisconnectCause.NotPrivilegedTarget);
                    Dialogs.Remove(dialog);
                    message.ChangeAsResponse(StatusCode.Forbidden);
                    sipNet.Send(message.Content, message.Via.EndPoint);
                    return;
                }
            }
            #endregion

            if (dialog.FromAccount.MaxConcurrentCalls == 1) // When call received from a softphone set its status to dialing
                dialog.FromAccount.Status = SipAccountStatus.Dialing;

            SendTemplateMessage(Template_Trying, message.Via.EndPoint, message.Via.Value, message.To.Value, message.From.Value, message.CallID, message.CSeq.Value);

            //if (Config.Default.IsForwardModeEnabled || message.ReferredBy != null) // in case of forward mode
            dialog.ToAccount = FindToAccount(message.To.Uri.UserID);
            if (dialog.ToAccount == null)
            {
                dialog.InviteMessage.To.Tag = Generate32bitRandomNumber();

                var ringingMessage = message.Clone() as SipMessage;
                ringingMessage.AppendRecordRoute();
                ringingMessage.ClearSdp();

                if (Accounts.Any(a => a.UserID == message.To.Uri.UserID)) // Check MatchRule
                {
                    Logger.WriteInfo("User '{0}' Found, but not registered or is busy.", message.To.Uri.UserID);
                    ringingMessage.ChangeAsResponse(StatusCode.Busy_Here);
                    dialog.Status = DialogStatus.UserBusySentWaitForAck;
                    sipNet.Send(ringingMessage.Content, ringingMessage.Via.EndPoint);
                }
                else
                {
                    // Target: IVR
                    ringingMessage.ChangeAsResponse(StatusCode.Ringing);
                    sipNet.Send(ringingMessage.Content, ringingMessage.Via.EndPoint);
                    IncommingCall(this, new IncommingCallEventArgs(dialog));
                }
            }
            else // Forward Mode
            {
                dialog.ToAccount.Status = SipAccountStatus.Dialing;
                if (dialog.ToAccount.MaxConcurrentCalls == 1)
                    message.To.DisplayName = message.CallID;
                ForwardInvite(message, dialog);
            }
        }

        bool CheckAndHandleInviteForFax(SipMessage message)
        {
            try
            {
                Logger.WriteInfo("Checking Invite for fax. MediaType={0}, Port={1}, Other={2}", message.Media.Type, message.Media.ClientPort, message.Media.Extra);

                if (message.To.Uri.UserID.Contains("UMSV-Informing"))
                    return false;

                if (message.Media.Type != SdpFieldMedia.MediaType.Image)
                    return false;

                Logger.WriteInfo("Fax Re-Invite: {0}", message.CallID);
                var dialog = FindDialog(message.CallID);

                string via = string.Join(",", message.Vias.Select(f => f.Value).ToArray());
                SendTemplateMessage(Template_FAXOK, dialog.InviteMessage.Via.EndPoint,
                                                    dialog.RtpNet.LocalPort,
                                                    dialog.DialogID,
                                                    via,
                                                    dialog.InviteMessage.From.Value,
                                                    dialog.InviteMessage.To.Value,
                                                    message.CSeq.Value,
                                                    dialog.InviteMessage.To.Uri.Value,
                                                    dialog.InviteMessage.From.Tag);

                dialog.Status = DialogStatus.FaxOKWaitForAck;
                dialog.MediaType = SdpFieldMedia.MediaType.Image;

                //ForwardInvite(message, dialog);

                //SendTemplateMessage(Template_Trying, message.Via.EndPoint, message.Via.Value, message.To.Value, message.From.Value, message.CallID, message.CSeq.Value);
                //message.ChangeAsResponse(StatusCode.Ok);
                //sipNet.Send(message.Content, message.Sender);

                //var client = Accounts.FirstOrDefault(a => a.UserID == "Dinstar"); //  && a.Status == SipAccountStatus.Idle
                //if (client != null)
                //    sipNet.Send(message.Content, client.SipEndPoint);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        bool CheckAndHandleInviteForPassThroughFax(SipMessage message)
        {
            try
            {
                Logger.WriteInfo("Checking Invite for pass-through fax. MediaType={0}, Port={1}, Other={2}", message.Media.Type, message.Media.ClientPort, message.Media.Extra);

                if (message.To.Uri.UserID.Contains("UMSV-Informing"))
                    return false;

                if (!message.Content.Contains("a=silenceSupp:off"))
                    return false;

                Logger.WriteInfo("(Pass-Through) Fax Re-Invite: {0}", message.CallID);
                var dialog = FindDialog(message.CallID);

                dialog.Extension = "pfax@" + dialog.Extension;

                StopPlayVoice(dialog);

                string via = string.Join(",", message.Vias.Select(f => f.Value).ToArray());
                SendTemplateMessage(Template_PASSTHROUGHFAXOK, dialog.InviteMessage.Via.EndPoint,
                                                    dialog.RtpNet.LocalPort,
                                                    dialog.DialogID,
                                                    via,
                                                    dialog.InviteMessage.From.Value,
                                                    dialog.InviteMessage.To.Value,
                                                    message.CSeq.Value,
                                                    dialog.InviteMessage.To.Uri.Value,
                                                    dialog.InviteMessage.From.Tag);

                dialog.Status = DialogStatus.FaxOKWaitForAck;
                //dialog.MediaType = SdpFieldMedia.MediaType.Image;

                //ForwardInvite(message, dialog);

                //SendTemplateMessage(Template_Trying, message.Via.EndPoint, message.Via.Value, message.To.Value, message.From.Value, message.CallID, message.CSeq.Value);
                //message.ChangeAsResponse(StatusCode.Ok);
                //sipNet.Send(message.Content, message.Sender);

                //var client = Accounts.FirstOrDefault(a => a.UserID == "Dinstar"); //  && a.Status == SipAccountStatus.Idle
                //if (client != null)
                //    sipNet.Send(message.Content, client.SipEndPoint);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        #endregion

        bool IsSame(IPEndPoint first, IPEndPoint second)
        {
            if (first == null || second == null)
                return first == second;

            return first.ToString() == second.ToString();
        }

        bool IsSame(IPAddress first, IPAddress second)
        {
            if (first == null || second == null)
                return first == second;

            return first.ToString() == second.ToString();
        }

        SipDialog FindDialog(SipMessage message)
        {
            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == message.CallID);
            if (dialog == null)
            {
                SendTransactionDoesNotExist(message);
                throw new DialogNotFoundException("Dialog '{0}' not found", message.CallID);
            }
            return dialog;
        }

        SipDialog FindDialog(string callID)
        {
            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == callID);
            if (dialog == null)
                throw new DialogNotFoundException("Dialog '{0}' not found", callID);
            else
                return dialog;
        }

        void HandleUnAuthorized(SipMessage unauthorizedMessage)
        {
            SipFieldWwwAuthenticate field = unauthorizedMessage.HeaderFields.First(h => h is SipFieldWwwAuthenticate) as SipFieldWwwAuthenticate;
            string digestURI = string.Format("sip:{0}", Config.Default.SipProxyEndPoint);

            Func<byte[], string> ToHex = ((pass2) => BitConverter.ToString(pass2).Replace("-", "").ToLower());
            string HA1 = SoftPhoneUsername + ":" + field.Realm + ":" + SoftPhonePassword;
            string HA2 = "REGISTER:" + digestURI;
            string responce = ComputeMd5Hash(ComputeMd5Hash(HA1) + ":" + field.Nonce + ":" + ComputeMd5Hash(HA2));

            var registerAuthorizationPhrase = string.Format("Authorization: Digest username=\"30\",realm=\"{1}\",nonce=\"{2}\",uri=\"{3}\",response=\"{4}\",algorithm=MD5\r\n",
                "", field.Realm, field.Nonce, digestURI, responce);

            Register(registerAuthorizationPhrase);
        }

        bool CheckForUnregister(SipMessage message, SipAccount account)
        {
            if (message.Contact.Uri.UserID == "*" || (message.Expires != null && message.Expires.Value == "0") || (message.Contact.Expires == "0"))
            {
                if (account == null)
                    Logger.WriteWarning("Unregister on not registered account.");
                else
                {
                    SessionManager.OnAccountUnRegister(account);
                    account.Status = SipAccountStatus.Offline;
                    Logger.WriteInfo("Account {0} Unregistered.", message.From.Uri.UserID);
                    if (UnRegistered != null)
                        UnRegistered(this, new UnRegisteredEventArgs() { Account = account });
                }
                return true;
            }

            return false;
        }

        SipAccount FindFromAccount(string userID, IPAddress sourceEndPoint)
        {
            var account = Accounts.FirstOrDefault(a => a.UserID == userID &&
                (Config.Default.AcceptCallFromNotRegisterUser || a.Status != SipAccountStatus.Offline));

            if (account != null)
                return account;

            #region Check if call originates from a Media Gateway (often in this case)

            account = Accounts.FirstOrDefault(a =>
                a.MaxConcurrentCalls != 1 && a.SipEndPoint != null && IsSame(a.SipEndPoint.Address, sourceEndPoint));

            if (account == null)
            {
                Logger.WriteWarning("From Account not found, no matching gateway, for number  '{0}' endpoint: '{1}', Gateway: '{2}'\r\nplease check if there is a gateway defined on static ip '{1}' or a gateway had been registered on this ip address at the system.", userID, sourceEndPoint,
                    string.Join(",", Accounts.Where(a => a.MaxConcurrentCalls != 1).Select(a => a.UserID + ":" + a.SipEndPoint)));
            }

            return account;

            #endregion
        }

        void ForwardInvite(SipMessage message, SipDialog dialog)
        {
            message.HeaderFirstLine.RequestHeader.MethodUri = new SipUri()
            {
                UserID = dialog.CalleeID,
                EndPoint = dialog.ToAccount.SipEndPoint,
            };

            message.HeaderFirstLine.RequestHeader.MethodUri.Rinstance = Generate32bitRandomNumber();

            dialog.ForwardInviteViaBranch = GenerateBranch(); // Save branch for pop in RINGING and OK
            message.PushVia(dialog.ForwardInviteViaBranch);
            message.AppendRecordRoute();

            if (message.ReferredBy != null)
            {
                dialog.DivertPartner = Dialogs.FirstOrDefault(d => (d.Status == DialogStatus.Refering || d.Status == DialogStatus.ReferingWaitForAccept) &&
                    (d.Partner.SipEndPoint.ToString() == dialog.Partner.SipEndPoint.ToString()));

                if (dialog.DivertPartner == null)
                    Logger.WriteError("TransferFrom is null for dialog:{0}", dialog.DialogID);
                else
                {
                    //if (dialog.TransferFrom.CallerID != dialog.TransferFrom.InviteMessage.From.Uri.UserID)
                    message.From.DisplayName = dialog.DivertPartner.CallerID;
                    //if (Config.Default.TransferMode == TransferMode.Attended)
                    //{
                    //    SipFieldReplaces field = new SipFieldReplaces();
                    //    field.Value = string.Format("{0};to-tag={2};from-tag={1}", dialog.DivertPartner.DialogID, 
                    //        dialog.DivertPartner.InviteMessage.To.Tag, dialog.DivertPartner.InviteMessage.From.Tag);
                    //    message.HeaderFields.Add(field);
                    //}
                }

                if (Config.Default.TransferMode == TransferMode.Blind && message.Supported != null)
                    message.HeaderFields.Remove(message.Supported);
            }

            #region Optional
            //if (!message.SdpFields.Any(f => f.Content == "a=direction:active"))
            //    message.SdpFields.Add(new SipSdpField("a=direction:active"));

            //if (!message.SdpFields.Any(f => f.Content == "a=nortpproxy:yes"))
            //    message.SdpFields.Add(new SipSdpField("a=nortpproxy:yes"));

            //message.HeaderFields.Add(new SipHeaderField()
            //{
            //    FieldName = "P-hint",
            //    Value = "usrloc applied",
            //});
            #endregion

            sipNet.Send(message.Content, dialog.ToAccount.SipEndPoint);
        }

        void rtpTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                var activeDialogs = Dialogs.Where(d => !d.IsForwardMode &&
                    d.Status == DialogStatus.Connect || d.Status == DialogStatus.Talking || d.Status == DialogStatus.ReferingWaitForAccept || d.Status == DialogStatus.WaitForDiverting).ToList();

                for (int index = 0; index < activeDialogs.Count(); index++)
                {
                    var dialog = activeDialogs[index];
                    if (dialog.VoiceStream != null && dialog.VoiceStreamOffset > -1 && dialog.VoiceStreamOffset < dialog.VoiceStream.Length - RtpChunkSize)
                        SendVoiceChunkStream(dialog);
                    else if (dialog.VoiceStreamOffset != -1)
                    {
                        dialog.VoiceStreamOffset = -1;

                        try
                        {
                            if (PlayFinished != null)
                                PlayFinished(this, new PlayFinishedEventArgs(dialog));
                        }
                        catch (Exception ex)
                        {
                            Logger.Write(ex, "rtpTimer_Tick OnPlayFinished");
                        }
                    }
                }


                foreach (var dialog in Dialogs.Where(d => d.Status == DialogStatus.Hold && !d.IsForwardMode))
                {
                    SendVoiceChunkStream(dialog, HoldVoice, dialog.HoldVoiceOffset);

                    dialog.HoldVoiceOffset = dialog.HoldVoiceOffset < HoldVoice.Length - RtpChunkSize ?
                        dialog.HoldVoiceOffset + RtpChunkSize : 0;
                }


            }
            catch (Exception ex)
            {
                Logger.Write(ex, "rtpTimer_Tick");
            }
        }

        void SendVoiceChunkStream(SipDialog dialog)
        {
            SendVoiceChunkStream(dialog, dialog.VoiceStream, dialog.VoiceStreamOffset);
            dialog.VoiceStreamOffset += RtpChunkSize;
        }

        void SendVoiceChunkStream(SipDialog dialog, byte[] voiceStream, int offset)
        {
            byte[] packet = new byte[RtpHeaderLength + RtpChunkSize];

            Array.Copy(new byte[] { VPXCC, M_PT_VoicePCMA }, 0, packet, 0, 2);
            Array.Copy(new byte[] { (byte)(dialog.Sequence >> 8), packet[3] = (byte)(dialog.Sequence) }, 0, packet, 2, 2);

            packet[4] = (byte)(dialog.TimeStamp >> 24);
            packet[5] = (byte)(dialog.TimeStamp >> 16);
            packet[6] = (byte)(dialog.TimeStamp >> 8);
            packet[7] = (byte)(dialog.TimeStamp);

            packet[8] = (byte)(dialog.SSRC >> 24);
            packet[9] = (byte)(dialog.SSRC >> 16);
            packet[10] = (byte)(dialog.SSRC >> 8);
            packet[11] = (byte)(dialog.SSRC);

            try
            {
                if (voiceStream.Length > offset + RtpChunkSize)
                {
                    Array.Copy(voiceStream, offset, packet, RtpHeaderLength, RtpChunkSize);

                    // To record Playing TEL number after disconnecting operator from the call
                    if (dialog.RecordedVoiceStartTime.HasValue && dialog.RecordedVoiceStartTime_Partner.HasValue && Config.Default.RecordPlayedTelephoneNumber)
                        dialog.RecordedVoiceStream_Partner.Write(packet, RtpHeaderLength, packet.Length - RtpHeaderLength);
                    // The Hold voice must be saved in operator voice since we always have hold for operator.
                    else if (dialog.RecordedVoiceStartTime.HasValue && dialog.DivertPartner != null && dialog.DivertPartner.RecordedVoiceStartTime.HasValue)
                    {
                        //Only Record played voice in operator Record (for HOLD or simultaneous voice with operator).
                        if (dialog.DialogType == DialogType.ClientIncomming || dialog.DialogType == DialogType.ClientOutgoing)
                        {
                            dialog.RecordedVoiceStream.Write(packet, RtpHeaderLength, packet.Length - RtpHeaderLength);
                            dialog.Hold_samples = dialog.Hold_samples + packet.Length - RtpHeaderLength;
                        }
                    }

                    //Sent voice to both sides
                    dialog.RtpNet.Send(packet);
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.WriteException("SendVoiceChunkStream, Socket disposed: {0}", dialog.DialogID);
            }
            catch (Exception ex)
            {
                Logger.WriteError("SendVoiceChunkStream error on dialog: {0}, message: {1}", dialog.DialogID, ex.Message);
            }

            dialog.Sequence++;
            dialog.TimeStamp += RtpChunkSize;
        }

        public void SendVoiceChunkStream(SipDialog dialog, byte[] voice)
        {
            byte[] packet = new byte[RtpHeaderLength + RtpChunkSize];

            Array.Copy(new byte[] { VPXCC, M_PT_VoicePCMA }, 0, packet, 0, 2);
            Array.Copy(new byte[] { (byte)(dialog.Sequence >> 8), packet[3] = (byte)(dialog.Sequence) }, 0, packet, 2, 2);

            packet[4] = (byte)(dialog.TimeStamp >> 24);
            packet[5] = (byte)(dialog.TimeStamp >> 16);
            packet[6] = (byte)(dialog.TimeStamp >> 8);
            packet[7] = (byte)(dialog.TimeStamp);

            packet[8] = (byte)(dialog.SSRC >> 24);
            packet[9] = (byte)(dialog.SSRC >> 16);
            packet[10] = (byte)(dialog.SSRC >> 8);
            packet[11] = (byte)(dialog.SSRC);

            try
            {
                Array.Copy(voice, 0, packet, RtpHeaderLength, RtpChunkSize);
            }
            catch (Exception ex)
            {
                Logger.WriteException("SendVoiceChunkStream, dialog:{0}, voice length: {1}, Message: {2}",
                    dialog.DialogID, voice.Length, ex.Message);
            }

            try
            {
                dialog.RtpNet.Send(packet);
            }
            catch (ObjectDisposedException)
            {
                Logger.WriteException("SendVoiceChunkStream, Socket disposed: {0}", dialog.DialogID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "SendVoiceChunkStream: {0}", dialog.DialogID);
            }

            dialog.Sequence++;
            dialog.TimeStamp += RtpChunkSize;
        }

        void RtpNet_OnReceive(RtpNet sender, byte[] packet)
        {
            /* When multiple packets of the same dialog are in the socket, multipple threads try to find relevant dialog. 
             *  For high loads this can leed to BUG, So the recording actions are written thread safe. 
             */
            var dialog = Dialogs.FirstOrDefault(d => d.RtpNet == sender);

            try
            {
                if (dialog == null)
                    return;
                /*  
                // Find SN and TS with bitconverter
                SeqNum[0] = packet[2];
                SeqNum[1] = packet[3];
                TimeSt[0] = packet[4];
                TimeSt[1] = packet[5];
                TimeSt[2] = packet[6];
                TimeSt[3] = packet[7];
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(SeqNum);
                    Array.Reverse(TimeSt);
                }
                SN = BitConverter.ToUInt16(SeqNum, 0);
                TS = BitConverter.ToUInt32(TimeSt, 0);
                */
                // Find SN and TS with arithmetic operations
                dialog.SN = (packet[2] >> 4) * 4096 + (packet[2] & 0x0f) * 256 + (packet[3] >> 4) * 16 + (packet[3] & 0x0f);
                dialog.TS = (packet[4] >> 4) * 268435456L + (packet[4] & 0x0f) * 16777216L + (packet[5] >> 4) * 1048576L + (packet[5] & 0x0f) * 65536 +
                            (packet[6] >> 4) * 4096 + (packet[6] & 0x0f) * 256 + (packet[7] >> 4) * 16 + (packet[7] & 0x0f);
                dialog.CurrentRemoteSSRC = (packet[8] >> 4) * 268435456L + (packet[8] & 0x0f) * 16777216L + (packet[9] >> 4) * 1048576L + (packet[9] & 0x0f) * 65536 +
                                           (packet[10] >> 4) * 4096 + (packet[10] & 0x0f) * 256 + (packet[11] >> 4) * 16 + (packet[11] & 0x0f);

                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                {
                    Logger.Write("RTP", "Receive(sec.ms):{0} {1}->{2}, len:{3}, SN:{4}, TS:{5}, {12}({6}-{7} {8}-{9}-{10}-{11})",
                           DateTime.Now.ToString("ss.ffff"), sender.RemoteEndPoint, sender.LocalEndPoint, packet.Length, dialog.SN, dialog.TS, packet[2], packet[3],
                           packet[4], packet[5], packet[6], packet[7], dialog.Hold_samples != 0 ? "HS:" + dialog.Hold_samples.ToString() + ", " : " ");
                }

                // Fill SSRC with the ssrc of the first RTP packet. The sender must fill SSRC of all RTP packets with this value.
                // By checking the SSRC we control unwanted or incorrect packets in the media stream and records.
                /*if (dialog.RemoteSSRC == 0)
                {
                    dialog.RemoteSSRC = dialog.CurrentRemoteSSRC;
                    Logger.Write("RTP", "{0}->{1} -> Set SSRC with:{2}, ({3}-{4}-{5}-{6})", sender.RemoteEndPoint, sender.LocalEndPoint, dialog.RemoteSSRC, packet[8], packet[9], packet[10], packet[11]);
                }*/

                CheckPakcketForDtmf(dialog, packet);

                if (dialog.MediaType == SdpFieldMedia.MediaType.Image)
                {
                    HandleFaxUdptl(dialog, packet);
                    return;
                }
                else if (dialog.MediaType == SdpFieldMedia.MediaType.Audio && !string.IsNullOrEmpty(dialog.Extension) && dialog.Extension.StartsWith("pfax"))
                {
                    HandlePassThroughFaxPacket(dialog, packet);
                    return;
                }

                if (dialog.DivertPartner != null && dialog.Status == DialogStatus.Talking) 
                {
                    dialog.DivertPartner.RtpNet.Send(packet);
                }

                if (dialog.Status == DialogStatus.Recording || dialog.Status == DialogStatus.Talking)
                {
                    lock (dialog.RecordedVoiceStreamSyncObject)
                    {
                        if (dialog.RecordedVoiceStream != null)
                        {
                            if (dialog.RecordedVoiceStartTime == null)
                            {
                                Logger.WriteView("Set RecordedVoiceStartTime for dialog:{0}, sender:{1}", dialog.DialogID, sender.RemoteEndPoint);
                                dialog.RecordedVoiceStartTime = DateTime.Now;
                            }

                            /* The First Time Stamp (FTS) is:
                             *      The first rtp packet of LEG-2
                             *      The first rtp packet of LEG-1 after connecting to LEG-2 (Status Talking || Recording )
                             */
                            if (dialog.FTS == 0 && packet.Length > 80)
                            {
                                dialog.FTS = dialog.TS;
                                // Do not record First packet of two legs - just initialize FTS and SSRC of their RTP session.

                                dialog.RemoteSSRC = dialog.CurrentRemoteSSRC;

                                Logger.WriteDebug("RTP - setting FTS and SSRC of the RTP session -> {0}->{1}, len:{2}, SN:{3}, TS:{4}, SSRC:{5} ({6}-{7} {8}-{9}-{10}-{11}  {12}-{13}-{14}-{15})",
                                        sender.RemoteEndPoint, sender.LocalEndPoint, packet.Length, dialog.SN, dialog.TS, dialog.RemoteSSRC, packet[2], packet[3], packet[4], packet[5], packet[6], packet[7], packet[8], packet[9], packet[10], packet[11]);
                            }
                            // SSRC must be unique. So we control un-wanted or currupted packets here 
                            else if (dialog.RemoteSSRC != 0 && dialog.RemoteSSRC != dialog.CurrentRemoteSSRC)
                            {
                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "# The SSRC of the RTP session is changed! SSRC:{0}-{1}-{2}-{3})", packet[8], packet[9], packet[10], packet[11]);
                            }
                            //RecordVoiceStream size is going to be larger than the RecordedVoiceStreamMaxLength, So dialog.TS - dialog.FTS means RecordedVoiceStream size after writing current packet and silent samples)
                            else if (dialog.TS - dialog.FTS > Constants.RecordedVoiceStreamMaxLength)
                            {
                                dialog.RecordedVoiceStream = null;
                                dialog.RecordedVoiceStream = new MemoryStream();
                                dialog.FTS = dialog.TS;
                                if (Config.Default.LogRtp)
                                    Logger.Write("RTP", "# Recorded Voice Stream is bigger than the maximum supported size so we create RecordedVoiceStream again. ({0}->{1} SSRC:{2}, SN:{3}, TS:{4}", sender.RemoteEndPoint, sender.LocalEndPoint, dialog.RemoteSSRC, dialog.SN, dialog.TS);

                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "# Recorded Voice Stream is bigger than the maximum supported size. ({0}->{1} SSRC:{2}, SN:{3}, TS:{4}", sender.RemoteEndPoint, sender.LocalEndPoint, dialog.RemoteSSRC, dialog.SN, dialog.TS);
                            }
                            // Do not record packets containing DTMF or empty RTP packet. Based on RFC47333 (RFC2833) size of DTMF payload is 4.
                            else if (packet.Length <= 4 + RtpHeaderLength)
                            {
                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "# Do not record packet with SN:{0} since it is DTMF signal", dialog.SN);
                            }
                            //Normal Packet
                            else if (dialog.TS - dialog.FTS - dialog.RecordedVoiceStream.Length + dialog.Hold_samples == packet.Length - RtpHeaderLength)
                            {
                                dialog.RecordedVoiceStream.Write(packet, RtpHeaderLength, packet.Length - RtpHeaderLength);

                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "# Normal packet SN:{0} ", dialog.SN);
                            }
                            //Packet-Loss, VAD 
                            else if (dialog.TS - dialog.FTS - dialog.RecordedVoiceStream.Length + dialog.Hold_samples > packet.Length - RtpHeaderLength)
                            {
                                dialog.Skipped_samples = dialog.TS - dialog.FTS - dialog.RecordedVoiceStream.Length + dialog.Hold_samples - packet.Length + RtpHeaderLength;

                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "#L# Adding {0} silent samples to the voice record of {1}->{2} - record len:{3}",
                                       dialog.Skipped_samples, sender.RemoteEndPoint, sender.LocalEndPoint, dialog.RecordedVoiceStream.Length);

                                //insert silent samples. The minimum resolution is 80 samples (half of the normal g711 rtp packet)
                                for (int ss = 0; ss < dialog.Skipped_samples / 80; ss++)
                                    dialog.RecordedVoiceStream.Write(silentSamples_80, 0, 80);

                                //Insert current packet
                                dialog.RecordedVoiceStream.Write(packet, RtpHeaderLength, packet.Length - RtpHeaderLength);
                            }
                            //Duplicate or previous packet
                            else
                            {
                                /* TODO: Rarely in heavy loads another thread gets resource sooner and take a packet from the last packets of the socket, but earlier
                                 *  packets is received by other threads later... We must insert this packet by finding the position of it in memoryStream.
                                 */
                                if (Config.Default.LogRtp && dialog.SN % 500 == 0)
                                    Logger.Write("RTP", "#D# Duplicate or backward packet with SN:{0} TS:{1} for {2}->{3}",
                                           dialog.SN, dialog.TS, sender.RemoteEndPoint, sender.LocalEndPoint);
                            }

                            /*if (Config.Default.LogRtp)
                                 Logger.Write("RTP", "#DEBUG# MemoryStream capacity:{0} length:{1} position:{2}", dialog.RecordedVoiceStream.Capacity, dialog.RecordedVoiceStream.Length, dialog.RecordedVoiceStream.Position);*/
                        }

                    }
                }

                //if (dialog.SN < dialog.PreviousSN)
                //    Logger.Write("RTP", "### Check this packet -> SN:{0} PreviousSN:{1} TS:{2} ", dialog.SN, dialog.PreviousSN, dialog.TS);

                if (dialog.DivertPartner != null && dialog.RemoteSSRC == dialog.CurrentRemoteSSRC) // && dialog.SN > dialog.PreviousSN )
                {
                    if (dialog.Status == DialogStatus.Talking) //  || dialog.Status == DialogStatus.Refering || dialog.Status == DialogStatus.ReferingWaitForAccept
                    {
                        //dialog.DivertPartner.RtpNet.Send(packet);
                        if (dialog.DivertPartner.AuditionEnabled)
                        {
                            dialog.DivertPartner.AuditionSentVoice = packet;
                            CheckForAuditionVoiceReady(dialog.DivertPartner);
                        }
                    }
                }

                // Remote entity must add one to SN for each RTP packet (in Packet-Loss more than one).
                //if ( dialog.SN > dialog.PreviousSN )
                //    dialog.PreviousSN = dialog.SN;

                if (dialog.AuditionEnabled)
                {
                    dialog.AuditionReceivedVoice = packet;
                    CheckForAuditionVoiceReady(dialog);
                }

            }
            catch (ObjectDisposedException)
            {
                Logger.WriteInfo("RtpNet_OnReceive, Socket disposed: {0}", dialog.DialogID);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "RtpNet_OnReceive: {0}", dialog.DialogID);
            }
        }

        void CheckForAuditionVoiceReady(SipDialog dialog)
        {
            if (dialog.AuditionSentVoice != null && dialog.AuditionReceivedVoice != null)
            {
                try
                {
                    byte[] voice = new byte[RtpHeaderLength + RtpChunkSize];
                    Array.Copy(dialog.AuditionSentVoice, 0, voice, 0, RtpHeaderLength);
                    var merge = Folder.Audio.AudioUtility.Mix(dialog.AuditionReceivedVoice.Skip(RtpHeaderLength).ToArray(), dialog.AuditionSentVoice.Skip(RtpHeaderLength).ToArray());
                    Array.Copy(merge, 0, voice, RtpHeaderLength, RtpChunkSize);

                    dialog.AuditionTarget.RtpNet.Send(voice);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, "Length:{0}, header:{1}, chunk:{2}", dialog.AuditionSentVoice.Length, RtpHeaderLength, RtpChunkSize);
                }

                dialog.AuditionReceivedVoice = null;
                dialog.AuditionSentVoice = null;
            }
        }

        void SipProxyNet_OnReceive(byte[] message, IPEndPoint remoteIp)
        {
            string textMessage = System.Text.Encoding.ASCII.GetString(message).Trim('\0');
            try
            {
                if (string.IsNullOrEmpty(textMessage) || textMessage == "\r\n\r\n")
                    return;

                if (!textMessage.StartsWith("CUSTOM") && Config.Default.LogSipMessage)
                    Logger.Write("sip", "[{0}] {1}", remoteIp, textMessage);

                SipMessage sipMessage = new SipMessage(textMessage)
                {
                    Sender = remoteIp
                };

                if (sipMessage.HeaderFirstLine.RequestHeader != null)
                    HandleSipRequest(sipMessage);
                else if (sipMessage.HeaderFirstLine.ResponseHeader != null)
                    HandleSipResponse(sipMessage);
                else
                    Logger.WriteWarning("Invalid SIP packet: {0}", textMessage);
            }
            catch (FormatException e)
            {
                Logger.Write(LogType.Error, "UnsupportedFieldFormatException: {0}\r\nmessage:{1}", e.Message, textMessage);
            }
            catch (DialogNotFoundException e)
            {
                Logger.WriteError(e.Message);
            }
        }

        void CheckPakcketForDtmf(SipDialog dialog, byte[] packet)
        {
            lock (dialog.DtmfDetectingSyncObject)
            {
                if (packet[PayloadTypeOffset] == M_PT_Dtmf && (packet[13] & 128) == 0 && dialog.DtmfDetecting == false)
                {
                    dialog.DtmfDetecting = true;
                    dialog.DtmfDetectingStartTime = DateTime.Now;
                }
                else if (packet[PayloadTypeOffset] != M_PT_Dtmf && DateTime.Now.Subtract(dialog.DtmfDetectingStartTime).TotalMilliseconds > Config.Default.DtmfDetectionInterval)
                {
                    dialog.DtmfDetecting = false;
                    return;
                }
                else
                    return;
            }

            char key = packet[12] < 10 ? (char)((byte)'0' + packet[12]) : (packet[12] == 10 ? '*' : '#');
            Logger.WriteDebug("DTMF detected: {0}", key);
            if (DtmfDetected != null)
                DtmfDetected(this, new DtmfDetectedEventArgs(dialog)
                {
                    Key = key
                });
        }

        SipDialog NewDialog(DialogStatus status, string callID, string callerID, string calleeID)
        {
            if (callID == null)
                callID = Guid.NewGuid().ToString() + "@" + Config.Default.SipProxyEndPoint.Address;
            Logger.WriteInfo("New Dialog {3}, CallID:{0}, callerID:{1}, calleeID:{2}", callID, callerID, calleeID, status);
            SipDialog dialog = new SipDialog(status, callID, callerID, calleeID, LocalAddress.Address);

            Dialogs.Add(dialog);
            dialog.StatusTimeout += dialog_StatusTimeout;
            return dialog;
        }

        void CheckAccountsTimeouts(object state)
        {
            try
            {
                var expiredAccounts = Accounts.Where(a =>
                    a.MaxConcurrentCalls == 1 &&
                    a.Status != SipAccountStatus.Offline &&
                    a.ExpireTime.HasValue &&
                    a.ExpireTime < DateTime.Now.AddSeconds(-Constants.ClientRegisterPeriod - 5)).ToList();

                foreach (var account in expiredAccounts)
                {
                    if (account.Status != SipAccountStatus.Idle && account.Status != SipAccountStatus.DND)
                    {
                        Logger.WriteWarning("Account '{0}' expired while is in {1} state.", account.UserID, account.Status);
                        account.ExpireTime = DateTime.Now.AddSeconds(Constants.ClientRegisterPeriod);
                    }
                    else
                    {
                        Logger.WriteWarning("Account '{0}' expired, last status: {1}", account.UserID, account.Status);
                        account.Status = SipAccountStatus.Offline;
                    }
                }

                //foreach (var dialog in Dialogs.Where(d => d.Status == DialogStatus.WaitForDiverting &&
                //    d.WaitForDivertingTimeout.HasValue && d.StatusChangeTime.AddSeconds(d.WaitForDivertingTimeout.Value) < DateTime.Now))
                //{
                //    Logger.WriteWarning("Dialog '{0}' on diverting timeouted, divert starting time: {1}", dialog.Call.DialogID, dialog.StatusChangeTime);
                //    dialog.Status = DialogStatus.Disconnected;
                //    dialog.Call.DisconnectCause = (int)DisconnectCause.NoAnswerFromUser;
                //    if (TransferFailed != null)
                //        TransferFailed(this, new TransferFailedEventArgs(dialog));
                //}
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "CheckAccountsTimeouts");
            }
            finally
            {
                if (checkAccountsTimeoutsTimer != null)
                    checkAccountsTimeoutsTimer.Change(Config.Default.CheckAccountsTimeoutsInterval, -1);
            }
        }

        void dialog_StatusTimeout(object sender, StatusTimeoutEventArgs e)
        {
            var dialog = (SipDialog)sender;
            try
            {
                switch (dialog.Status)
                {
                    case DialogStatus.InviteWaitForAck:
                        HandleCallDisconnected(dialog, DisconnectCause.SubscriberAbsent);
                        break;

                    case DialogStatus.Connect: // new added
                    case DialogStatus.Talking:
                    case DialogStatus.Hold:
                        Logger.WriteError("Timeout on status: {0}", dialog.Status);
                        DisconnectCall(dialog);
                        break;

                    case DialogStatus.Invite:
                    case DialogStatus.Ringing:
                        dialog.Status = DialogStatus.CancelingWaitForAck;
                        if (dialog.ToAccount != null)
                            SendTemplateMessage(Template_Cancel, dialog.ToAccount.SipEndPoint, dialog.InviteMessage.From.Value, dialog.CalleeID, dialog.DialogID, dialog.InviteMessage.Via.Value); // Cancel Reffering
                        dialog.InviteMessage.PopVia(dialog.ForwardInviteViaBranch);

                        if (dialog.DivertPartner != null)
                        {
                            HandleCallDisconnected(dialog, DisconnectCause.NoAnswerFromUser);
                            //dialog.SaveCall(DisconnectCause.NoAnswerFromUser);
                            //OnTransferFailed(this, new TransferFailedEventArgs(dialog));
                        }
                        else
                        {
                            if (IsSoftPhoneMode)
                            {
                            }
                            else
                                SendTemplateMessage(Template_BusyHere, dialog.FromAccount.SipEndPoint, dialog.InviteMessage.Via.Value, dialog.InviteMessage.To.Value, dialog.InviteMessage.From.Value, dialog.DialogID);
                        }
                        break;

                    case DialogStatus.ByingWaitForOk:
                        if (IsSoftPhoneMode)
                            SendTemplateMessage(Template_Cancel, Config.Default.SipProxyEndPoint, dialog.InviteMessage.From.Value, dialog.CalleeID, dialog.DialogID, dialog.InviteMessage.Via.Value);
                        else
                        {
                            if (dialog.SendByeRetry < Constants.RetrySendSipMessageMaxTimes)
                            {
                                dialog.SendByeRetry++;
                                sipNet.Send(dialog.ByeMessage, dialog.ByeTarget);
                            }
                            else
                                HandleCallDisconnected(dialog, DisconnectCause.NormalUnspecified);
                        }
                        break;

                    case DialogStatus.ReferingWaitForAccept:
                        Logger.WriteTodo("handle DialogStatus.ReferingWaitForAccept");
                        break;

                    case DialogStatus.Refering:
                        Logger.WriteWarning("Timeout on status: {0}", dialog.Status);
                        DisconnectCall(dialog);
                        break;


                    case DialogStatus.Recording:
                        Logger.WriteWarning("Timeout on status: {0}", dialog.Status);

                        // I thisk DisconnectedDialogsRecordedVoices is useless, if you find its usage tell me!!!
                        if (DisconnectedDialogsRecordedVoices[dialog.DialogID] == null)
                            lock (DisconnectedDialogsRecordedVoices)
                            {
                                DisconnectedDialogsRecordedVoices[dialog.DialogID] = dialog.RecordedVoiceStream.ToArray();
                            }
                        //////////////////////

                        DisconnectCall(dialog);
                        break;

                    case DialogStatus.WaitForDiverting:
                        CancelDialog(dialog);
                        break;

                    case DialogStatus.CancelingWaitForAck:
                        Logger.WriteInfo("Check timeout for CancelingWaitForAck, callID:{0}", dialog.DialogID);
                        HandleCallDisconnected(dialog, DisconnectCause.NormalCallClearing);
                        //ReleaseDialog(dialog);
                        break;

                    case DialogStatus.DialingWaitForOk:
                        if (dialog.DivertPartner != null)
                        {
                            dialog.DivertPartner.DivertPartner = null;
                            Dialogs.Remove(dialog);
                            TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                        }
                        else
                            HandleCallDisconnected(dialog, DisconnectCause.SubscriberAbsent);
                        break;

                    case DialogStatus.Dialing:
                        if (dialog.ToAccount == null) { Logger.WriteException("Timeout on status dialing, while ToAccount is null, dialog: '{0}'", dialog.DialogID); Dialogs.Remove(dialog); break; }
                        if (dialog.SendInviteRetry < Constants.RetrySendSipMessageMaxTimes)
                        {
                            dialog.SendInviteRetry++;
                            Logger.WriteDebug("Resend invite '{0}' time: {1}", dialog.DialogID, dialog.SendInviteRetry);
                            dialog.Status = DialogStatus.Dialing;
                            sipNet.Send(dialog.InviteMessage.Content, dialog.ToAccount.SipEndPoint);
                        }
                        else
                        {
                            if (dialog.ToAccount.MaxConcurrentCalls == 1)
                            {
                                if (dialog.ToAccount == null)
                                    Logger.WriteWarning("ToAccount on timeout status dialing is null, dialogID:{9}", dialog.DialogID);
                                else
                                    Logger.WriteWarning("Account {0} doesn't response to invite after {1} times, setting it Offline.", dialog.ToAccount.UserID, Constants.RetrySendSipMessageMaxTimes);
                                dialog.ToAccount.Status = SipAccountStatus.Offline;
                            }

                            if (dialog.DivertPartner != null)
                            {
                                Dialogs.Remove(dialog);
                                dialog.DivertPartner.DivertPartner = null;
                                TransferFailed(this, new TransferFailedEventArgs(dialog.DivertPartner));
                            }
                            else
                                HandleCallDisconnected(dialog, DisconnectCause.SubscriberAbsent);
                        }
                        break;

                    case DialogStatus.Disconnected:
                        if ((dialog.AuditionTarget != null) || (dialog.DivertPartner != null && dialog.DivertPartner.AuditionTarget != null))
                            DisconnectEavesdropperDialog(dialog);
                        Dialogs.Remove(dialog);
                        break;

                    case DialogStatus.DivertingWaitForTargetResponse:
                        Logger.WriteWarning("DivertingWaitForTargetResponse timeout, callid: {0}, target dialog: {1}", dialog.DialogID, dialog.DivertPartner != null ? dialog.DivertPartner.DialogID : null);
                        dialog.Status = DialogStatus.Connect;
                        if (dialog.DivertPartner != null)
                        {
                            CancelDialog(dialog.DivertPartner);
                            TransferFailed(this, new TransferFailedEventArgs(dialog));
                        }
                        else
                        {
                            Logger.WriteWarning("Removing dialog:{0} because of DivertingWaitForTargetResponse timeout and timeout more than 10 minutes.", dialog.Call.ID);
                            CancelDialog(dialog);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "removing dialog from dialogs");

                try
                {
                    Dialogs.Remove(dialog);
                }
                catch
                {
                }
            }
        }

        IPEndPoint FindPartner(SipDialog dialog)
        {
            if (dialog.Partner.SipEndPoint != null)
                return dialog.Partner.SipEndPoint;

            if (dialog.Partner.UserID == dialog.InviteMessage.From.Uri.UserID)
            {
                int port = 5060;
                if (dialog.InviteMessage.Contact.Uri.Port.HasValue)
                    port = dialog.InviteMessage.Contact.Uri.Port.Value;

                return new IPEndPoint(IPAddress.Parse(dialog.InviteMessage.Contact.Uri.Address), port);
            }


            return null;
        }

        void CheckMessagesTimeouts(object state)
        {
            try
            {
                foreach (var dialog in Dialogs)
                    dialog.CheckForStatusTimeout();
            }
            finally
            {
                if (checkMessagesTimeoutsTimer != null)
                    checkMessagesTimeoutsTimer.Change(Config.Default.CheckMessagesTimeoutsInterval, -1);
            }
        }

        void CheckInviteWaitForAckDialogsTimeouts(object state)
        {
            try
            {
                foreach (var dialog in Dialogs)
                    if (dialog.Status == DialogStatus.InviteWaitForAck && dialog.SendOKRetry <= 3)
                    {
                        string via = string.Join(",", dialog.InviteMessage.Vias.Select(f => f.Value).ToArray());
                        SendTemplateMessage(Template_Ok, dialog.InviteMessage.Via.EndPoint,
                                                         dialog.RtpNet.LocalPort,
                                                         dialog.DialogID,
                                                         via,
                                                         dialog.InviteMessage.From.Value,
                                                         dialog.InviteMessage.To.Value,
                                                         dialog.InviteMessage.CSeq.Value,
                                                         dialog.InviteMessage.To.Uri.Value,
                                                         dialog.InviteMessage.From.Tag);

                        dialog.SendOKRetry++;
                    }
            }
            catch (Exception ex)
            {
                Logger.WriteError("An Exception occured during checking InviteWaitForAck Dialogs....");
                Logger.Write(ex);
            }
            finally
            {
                if (checkInviteWaitForAckDialogsTimeoutsTimer != null)
                    checkInviteWaitForAckDialogsTimeoutsTimer.Change(500, -1);
            }
        }

        void HandleCallDisconnected(SipDialog dialog, DisconnectCause disconnectCause)
        {
            if (dialog.Call.DisconnectCause == 0)
            {
                Logger.WriteDebug("HandleCallDisconnected -> DialogID:{0} disconnected with disconnectCause:{1}({2})", dialog.DialogID, disconnectCause.ToString(), (int)disconnectCause);
                dialog.Call.DisconnectCause = (int)disconnectCause;
            }
            else if ((dialog.Call.DisconnectCause == 16 && (int)disconnectCause == 16) || (dialog.Call.DisconnectCause == 17 && (int)disconnectCause == 17) ||
                     (dialog.Call.DisconnectCause == 31 && (int)disconnectCause == 31) || (dialog.Call.DisconnectCause == 19))
            {
                if (dialog.Call.DisconnectCause == 19)
                {
                    Logger.WriteDebug("HandleCallDisconnected -> Disconnect Cause is set for un-answer call({0}) previously.", dialog.DialogID);
                    dialog.Call.DisconnectCause = 19; //(int)DisconnectCause.NoAnswerFromUser;
                }
                else
                    Logger.WriteDebug("HandleCallDisconnected -> Duplicate DisconnectCause:{1}({2}) for call({0}) - The main reason is retransmission", dialog.DialogID, disconnectCause.ToString(), (int)disconnectCause);

                //FIXME: Same disconnectCause is necessary someTimes! Otherwise 
                //if (CallDisconnected != null)
                //    CallDisconnected(this, new CallDisconnectedEventArgs(dialog));

                // OurPolicy: Set DisconnectCause only one time for each dialog (Consider first DisconnectCause if retransmission occured). 
                //return;
            }
            else
            {
                if (dialog.Call.DisconnectCause == 16 && (int)disconnectCause == 31)
                    Logger.WriteDebug("HandleCallDisconnected -> Subscriber disconnected first, so changing Disconnect cause from {0} to {1}({2}) for callID:{3}  ", dialog.Call.DisconnectCause, (int)disconnectCause, disconnectCause, dialog.DialogID);
                else
                    Logger.WriteDebug("HandleCallDisconnected -> Change disconnect cause from {0} to {1}({2}) for callID:{3} ", dialog.Call.DisconnectCause, (int)disconnectCause, disconnectCause, dialog.DialogID);

                dialog.Call.DisconnectCause = (int)disconnectCause;
            }

            if (CallDisconnected != null)
                CallDisconnected(this, new CallDisconnectedEventArgs(dialog));

            dialog.Disconnected(disconnectCause);
        }

        bool CheckAndHandleInviteForHold(SipMessage inviteMessage)
        {
            var user = Accounts.FirstOrDefault(a => a.UserID == inviteMessage.From.Uri.UserID);
            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == inviteMessage.CallID);

            //if (inviteMessage.To.Uri.UserID.Contains("UMSV-Informing"))
            //    return false;

            if (dialog == null)
                return false;

            if (inviteMessage.RtpAddress == "0.0.0.0" || inviteMessage.SdpFields.Any(s => s.Value.ToLower() == "sendonly" || s.Value.ToLower() == "inactive" || s.Value.ToLower() == "receiveonly"))
            {
                Logger.WriteDebug("Hold on dialog {0}", inviteMessage.CallID);
                inviteMessage.RtpAddress = "0.0.0.0"; // In some cases we receive sendonly and target can not reconginize this
                inviteMessage.SdpFields.Add(new SdpFieldMediaAttribute("sendonly"));

                if (user != null)
                    user.Status = SipAccountStatus.Hold;
                dialog.Status = DialogStatus.Hold;
                dialog.HoldVoiceOffset = 0;
                if (dialog.DivertPartner != null)
                {
                    dialog.DivertPartner.Status = DialogStatus.Hold;
                    dialog.DivertPartner.HoldVoiceOffset = 0;
                }
            }
            else
            {
                if (dialog.Status == DialogStatus.Hold)
                {
                    if (user != null)
                        user.Status = SipAccountStatus.Talking;
                    dialog.Status = DialogStatus.Talking;
                    if (dialog.DivertPartner != null)
                        dialog.DivertPartner.Status = DialogStatus.Talking;
                }
                else
                    return true;
            }

            if (dialog.IsForwardMode)
                ForwardMessage(inviteMessage, dialog);
            else
            {
                var okMessage = inviteMessage.Clone() as SipMessage;
                okMessage.ChangeAsResponse(StatusCode.Ok);
                okMessage.Contact.Uri = okMessage.To.Uri;
                if (okMessage.Route != null)
                {
                    okMessage.HeaderFields.Add(new SipFieldRecordRoute()
                    {
                        Value = okMessage.Route.Value,
                    });
                    okMessage.RemoveField(okMessage.Route);
                }

                okMessage.SdpFieldSessionOrigin.ClientAddress = Config.Default.SipProxyAddress;
                okMessage.RtpAddress = Config.Default.SipProxyAddress;
                okMessage.RtpPort = dialog.RtpNet.LocalPort;
                sipNet.Send(okMessage.Content, okMessage.Via.EndPoint);
            }

            return true;
        }

        void ForwardMessage(SipMessage message, SipDialog dialog)
        {
            try
            {
                if (dialog.FromAccount == null || dialog.ToAccount == null)
                {
                    Logger.WriteError("Error in ForwardMessage FromAccount: {0} ToAccount: {1}", dialog.FromAccount, dialog.ToAccount);
                    return;
                }

                if (IsSame(message.Sender, dialog.FromAccount.SipEndPoint))
                    sipNet.Send(message.Content, dialog.ToAccount.SipEndPoint);
                else if (IsSame(message.Sender, dialog.ToAccount.SipEndPoint))
                    sipNet.Send(message.Content, dialog.FromAccount.SipEndPoint);
                else // Often for CISCO which receive and send port are not same.
                {
                    if (IsSame(message.Sender.Address, dialog.FromAccount.SipEndPoint.Address))
                        sipNet.Send(message.Content, dialog.ToAccount.SipEndPoint);
                    else
                        sipNet.Send(message.Content, dialog.FromAccount.SipEndPoint);
                }
            }
            catch (NullReferenceException)
            {
                Logger.WriteError("ForwardMessage NullReference Error, dialog: {1}, message: {0}", message.Content, dialog.DialogID);
            }
        }
    }

    #region Event Args

    public class MessageArrivedEventArgs : EventArgs
    {
        public MessageArrivedEventArgs(string message)
        {
            this.Message = message;
        }

        public string Message { get; set; }
    }

    public class IncommingCallEventArgs : OnDialogEventArgs
    {
        public IncommingCallEventArgs(SipDialog dialog)
            : base(dialog)
        {
        }
    }

    public class CallDisconnectedEventArgs : OnDialogEventArgs
    {
        public CallDisconnectedEventArgs(SipDialog dialog)
            : base(dialog)
        {
        }
    }

    //public class CallDisconnectedEventArgs : EventArgs
    //{
    //    public CallDisconnectedEventArgs(System.Threading.Thread containingThread)
    //    {
    //        this.ContainingThread = containingThread;
    //    }

    //    public System.Threading.Thread ContainingThread;
    //}

    public class TransferFailedEventArgs : OnDialogEventArgs
    {
        public TransferFailedEventArgs(SipDialog dialog)
            : base(dialog)
        {
        }
    }

    public abstract class OnDialogEventArgs : EventArgs
    {
        public OnDialogEventArgs(SipDialog dialog)
        {
            this.Dialog = dialog;
        }

        public SipDialog Dialog;
    }

    public class CallStablishedEventArgs : OnDialogEventArgs
    {
        public CallStablishedEventArgs(SipDialog dialog)
            : base(dialog)
        {
        }
    }

    public class TargetRingingEventArgs : OnDialogEventArgs
    {
        public TargetRingingEventArgs(SipDialog dialog)
            : base(dialog)
        {

        }
    }

    public class PlayFinishedEventArgs : OnDialogEventArgs
    {
        public PlayFinishedEventArgs(SipDialog dialog)
            : base(dialog)
        {

        }
    }

    public class TimeoutEventArgs : OnDialogEventArgs
    {
        public TimeoutEventArgs(SipDialog dialog)
            : base(dialog)
        {

        }
    }

    public class DtmfDetectedEventArgs : OnDialogEventArgs
    {
        public DtmfDetectedEventArgs(SipDialog dialog)
            : base(dialog)
        {

        }
        public char Key;
    }

    public class RegisteredEventArgs : EventArgs
    {
        public RegisteredEventArgs(SipAccount account)
        {
            this.Account = account;
        }

        public SipAccount Account;
    }

    public class UnRegisteredEventArgs : EventArgs
    {
        public UnRegisteredEventArgs()
        {
        }

        public SipAccount Account;
    }

    #endregion
}

