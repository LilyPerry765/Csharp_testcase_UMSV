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

namespace UMSV
{
    public partial class SipService
    {
        public event EventHandler<IncommingCallEventArgs> IncommingCall;
        public event EventHandler<RegisteredEventArgs> Registered;
        public event EventHandler<UnRegisteredEventArgs> UnRegistered;
        public event EventHandler<CallDisconnectedEventArgs> CallDisconnected;
        public event EventHandler<TransferFailedEventArgs> TransferFailed;
        public event EventHandler<CallStablishedEventArgs> CallStablished;
        public event EventHandler<CallStablishedEventArgs> ScheduledOutcallStablished;
        public event EventHandler<CallStablishedEventArgs> FaxStablished;
        public event EventHandler<DtmfDetectedEventArgs> DtmfDetected;
        public event EventHandler<PlayFinishedEventArgs> PlayFinished;
        public event EventHandler<TargetRingingEventArgs> OnTargetRinging;

        IPEndPoint LocalAddress;

        public bool Start(IPEndPoint localAddress)
        {
            try
            {
                sipNet = new SipNet();
                this.LocalAddress = localAddress;

                if (!sipNet.Start(localAddress))
                    return false;

                ReloadGateways();
                SessionManager.Start();

                sipNet.OnReceive += new SipPacketTransmitEventHandler(SipProxyNet_OnReceive);
                checkMessagesTimeoutsTimer = new System.Threading.Timer(CheckMessagesTimeouts, null, Config.Default.CheckMessagesTimeoutsInterval, -1);
                checkAccountsTimeoutsTimer = new System.Threading.Timer(CheckAccountsTimeouts, null, Config.Default.CheckAccountsTimeoutsInterval, -1);
                checkInviteWaitForAckDialogsTimeoutsTimer = new System.Threading.Timer(CheckInviteWaitForAckDialogsTimeouts, null, 500, -1);

                HoldVoice = Voice.GetByName(Constants.Voice_Hold);
                if (HoldVoice == null)
                    HoldVoice = new byte[] { };

                rtpTimer.Tick += new EventHandler(rtpTimer_Tick);
                rtpTimer.Period = RtpInterval;
                rtpTimer.Stopped += new EventHandler(rtpTimer_Stopped);
                rtpTimer.Disposed += new EventHandler(rtpTimer_Disposed);
                rtpTimer.Start();
                Logger.WriteView("rtpTimer started!");

                VoiceFileManager.Start();
                Logger.WriteDebug("SipService Started!");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public void ReloadGateways()
        {
            try
            {
                Logger.WriteInfo("ReloadGateways ...");
                MediaGatewayConfig.Load();
                Accounts.RemoveAll(a => a.MaxConcurrentCalls != 1);

                foreach (var cisco in MediaGatewayConfig.Default.Ciscos)
                {
                    if (string.IsNullOrEmpty(cisco.Address))
                    {
                        Logger.WriteImportant("Gateway {0} has no address.", cisco.UserID);
                        continue;
                    }

                    SipAccount account = new SipAccount()
                    {
                        MatchRule = cisco.MatchRule,
                        MaxConcurrentCalls = MediaGatewayConfig.Default.Links.Count(l => l.DeviceID == cisco.DeviceID && l.IsEnabled) * 30,
                        Status = SipAccountStatus.Idle,
                        UserID = cisco.UserID,
                        ExpireTime = DateTime.Now.AddYears(1),
                    };
                    Logger.WriteDebug("ReloadGateway {0}@{1} configs... MaxConcurrentCalls is set to {2} ", cisco.UserID, cisco.Address, account.MaxConcurrentCalls);

                    //Hordcoded cisco emulator for test purposes
                    if (cisco.UserID == "sipp_gateway")
                    {
                        account.MaxConcurrentCalls = 500;
                        Logger.WriteDebug("Changing {0}@{1} MaxConcurrentCalls to {2}. The sipp_gateway name is reserved for emulation and test purposes ", cisco.UserID, cisco.Address, account.MaxConcurrentCalls);
                    }


                    if (!cisco.Address.Contains(":"))
                        account.SipEndPoint = new IPEndPoint(IPAddress.Parse(cisco.Address), 5060);
                    else
                        account.SipEndPoint = new IPEndPoint(IPAddress.Parse(cisco.Address.Split(':').First()), int.Parse(cisco.Address.Split(':').Last()));

                    Accounts.Add(account);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteCritical("Error in ReloadGateways: {0}, trace: {1}", ex.Message, ex.StackTrace);
            }
        }

        void rtpTimer_Disposed(object sender, EventArgs e)
        {
            Logger.WriteWarning("rtpTimer_Disposed");
        }

        void rtpTimer_Stopped(object sender, EventArgs e)
        {
            Logger.WriteWarning("rtpTimer_Stopped");
        }

        public void Register(string registerAuthorizationPhrase)
        {
            SendRegister(Config.Default.SipProxyEndPoint, 1, 60, LocalAddress, SoftPhoneUsername, registerAuthorizationPhrase, string.Empty);
        }

        public void UnRegister(string registerAuthorizationPhrase)
        {
            SendRegister(Config.Default.SipProxyEndPoint, 1, 60, LocalAddress, SoftPhoneUsername, registerAuthorizationPhrase, ";expires=0");
        }

        public void Stop()
        {
            try
            {
                SessionManager.Stop();
                VoiceFileManager.Stop();
                rtpTimer.Stop();
                rtpTimer.Dispose();

                checkMessagesTimeoutsTimer = null;
                checkAccountsTimeoutsTimer = null;
            }
            catch
            {
            }
        }

        public void DisconnectCall(SipDialog dialog)
        {
            try
            {
                Logger.WriteDebug("DisconnectDialog callid:{0} callerID:{1}", dialog.DialogID, dialog.CallerID);
                dialog.ByeTarget = FindPartner(dialog);

                //condition for informing call
                //if (dialog.CallerID.StartsWith("UMS"))
                if (dialog.IsInforming)
                    dialog.ByeMessage = SendBye(dialog.ByeTarget, dialog.DialogID, dialog.InviteMessage.To.Value, dialog.InviteMessage.From.Value, dialog.InviteMessage.From.Tag, dialog.Partner.UserID, dialog.Extension, dialog.DialogType);
                else
                    dialog.ByeMessage = SendBye(dialog.ByeTarget, dialog.DialogID, dialog.InviteMessage.From.Value, dialog.InviteMessage.To.Value, dialog.InviteMessage.From.Tag, dialog.Partner.UserID, dialog.Extension, dialog.DialogType);

                dialog.Status = DialogStatus.ByingWaitForOk;
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "DisconnectCall");
            }
        }

        public void SoftPhoneDisconnectCall(SipDialog dialog)
        {
            try
            {
                Logger.WriteDebug("DisconnectDialog callid:{0}", dialog.DialogID);
                SendTemplateMessage(Template_SoftphoneBye, Config.Default.SipProxyEndPoint, dialog.DialogID, dialog.InviteMessage.From.Value, dialog.InviteMessage.To.Value,
                    dialog.InviteMessage.From.Tag, dialog.InviteMessage.To.Uri.UserID, LocalAddress, SoftPhoneUsername);
                dialog.Status = DialogStatus.ByingWaitForOk;
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "DisconnectCall");
            }
        }

        internal void DisconnectCallOnTransferMode(SipDialog dialog)
        {
            try
            {
                if (dialog.Status == DialogStatus.Disconnected)
                    return;
                Logger.WriteDebug("DisconnectCallOnTransferMode callid:{0}", dialog.DialogID);
                dialog.ByeTarget = FindPartner(dialog);
                dialog.ByeMessage = SendBye(dialog.ByeTarget, dialog.DialogID, dialog.InviteMessage.To.Value, dialog.InviteMessage.From.Value, dialog.InviteMessage.From.Tag, dialog.Partner.UserID, dialog.Extension, dialog.DialogType);
                dialog.Status = DialogStatus.ByingWaitForOk;
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "DisconnectCall");
            }
        }

        internal void CancelDialog(SipDialog dialog)
        {
            try
            {
                Logger.WriteDebug("CancelDialog callid:{0}", dialog.DialogID);
                var partner = FindPartner(dialog);
                dialog.InviteMessage.HeaderFirstLine.RequestHeader.Method = SipMethod.CANCEL;
                dialog.InviteMessage.ClearSdp();
                dialog.InviteMessage.CSeq.Method = SipMethod.CANCEL;
                dialog.InviteMessage.CSeq.Number = 2;
                sipNet.Send(dialog.InviteMessage.Content, partner);

                dialog.Status = DialogStatus.CancelingWaitForAck;
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "DisconnectCall");
            }
        }

        internal void CancelDialog(SipDialog dialog, IPEndPoint endPoint)
        {
            try
            {
                Logger.WriteDebug("CancelDialog callid:{0}", dialog.DialogID);
                var partner = FindPartner(dialog);
                dialog.InviteMessage.HeaderFirstLine.RequestHeader.Method = SipMethod.CANCEL;
                dialog.InviteMessage.ClearSdp();
                dialog.InviteMessage.CSeq.Method = SipMethod.CANCEL;
                dialog.InviteMessage.CSeq.Number = 2;
                sipNet.Send(dialog.InviteMessage.Content, partner);

                dialog.Status = DialogStatus.CancelingWaitForAck;
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "DisconnectCall");
            }
        }

        //internal void CancelCall(SipDialog dialog)
        //{
        //    try
        //    {
        //        Logger.WriteDebug("CancelCall callid:{0}", dialog.CallID);
        //        var partner = FindPartner(dialog);
        //        SendTemplateMessage("CANCEL", partner, dialog.InviteMessage.From.Value, dialog.CalleeID, dialog.CallID, dialog.InviteMessage.Via.Value);
        //        dialog.Status = DialogStatus.CancelingWaitForAck;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Write(ex, "CancelCall");
        //    }
        //}

        internal SipAccount FindToAccount(string userID)
        {
            var accounts = from a in Accounts
                           where (a.MaxConcurrentCalls == 1 ? a.Status == SipAccountStatus.Idle : a.Status != SipAccountStatus.Offline) &&
                                (a.MaxConcurrentCalls == 0 || Dialogs.Count(d => d.ToAccount == a) < a.MaxConcurrentCalls)
                           orderby a.LastCallTime
                           select a;

            var account = accounts.FirstOrDefault(a => a.UserID == userID);
            if (account == null)
                account = accounts.FirstOrDefault(a => !string.IsNullOrEmpty(a.MatchRule) && Regex.IsMatch(userID, a.MatchRule));

            return account;
        }

        internal SipDialog Dial(string callerID, string calleeID, SipAccount toAccount, string referByUserID, string graphTrack, string replaces = null)
        {
            toAccount.LastCallTime = DateTime.Now;
            SipDialog dialog = NewDialog(DialogStatus.Dialing, null, callerID, toAccount.UserID);
            dialog.DialogType = toAccount.MaxConcurrentCalls == 1 ? DialogType.ClientIncomming : DialogType.GatewayOutgoing;
            dialog.RtpNet = new RtpNet(LocalAddress.Address);
            dialog.ToAccount = toAccount;

            SendInvite(dialog, callerID, calleeID, dialog.RtpNet.LocalPort,
                dialog.DialogID, dialog.ToAccount.SipEndPoint, string.Empty, graphTrack, replaces);

            return dialog;
        }

        //defined for avoiding many lines of extra log when maximum capacity reaches in cisco
        private static bool maximumCapacityLogFlag = true;

        public SipDialog StartIVROutCall(string callerId, string calleeId, DivertTarget target)
        {
            //this label is added in order to understand wich outcall is in informing mode.
            //callerId = "UMSV-Informing" + callerId;

            //Logger.WriteInfo(callerId + " ---> " + calleeId);

            //List<DivertTarget> targets = new List<DivertTarget>();
            //foreach (SipAccount account in InformingDivertTargets)
            //{
            //    if (Regex.IsMatch(calleeId, account.MatchRule))
            //        targets.Add(new DivertTarget(account, calleeId));
            //}

            //choosing Cisco randomly
            //DivertTarget target = targets[new Random().Next(0, targets.Count)];
            //Logger.WriteTodo("targets: {0}, target: {1}", targets.Count, target.Account.UserID);
            //Logger.WriteTodo("count: {0}", Dialogs.Count(t => t.CalleeID == target.Account.UserID/*outgoing*/ || t.DialogID.EndsWith(MediaGatewayConfig.Default.Ciscos.Where(c => c.UserID == target.Account.UserID).Single().Address)/*incoming*/));
            //Logger.WriteTodo("max concurrent; {0}", target.Account.MaxConcurrentCalls);

            //this condition checks current incoming out outgoing calls of selected cisco account
            if (SipService.Default.Dialogs.Count(t => t.CalleeID == target.Account.UserID/*outgoing*/ || t.DialogID.EndsWith(MediaGatewayConfig.Default.Ciscos.Where(c => c.UserID == target.Account.UserID).Single().Address)/*incoming*/) >= target.Account.MaxConcurrentCalls)
            {
                if (maximumCapacityLogFlag)
                {
                    Logger.WriteImportant("Maximum capacity reached in current Cisco account: {0}", target.Account.UserID);
                    maximumCapacityLogFlag = false;
                }
                return null;
            }
            maximumCapacityLogFlag = true;

            SipDialog targetDialog = Dial(callerId, target.Phone, target.Account, string.Empty, string.Empty);
            targetDialog.IsInforming = true;
            targetDialog.CurrentlyDivertTarget = target;

            if (targetDialog.DialogType == DialogType.GatewayOutgoing || targetDialog.DialogType == DialogType.ClientOutgoing)
            {
                //In order to generate proper ACK for Cisco during outcall session initiation.
                //For more information review the comment inside SendAck method.
                targetDialog.Extension = target.Phone;
                Logger.WriteImportant("Setting target dialog extension with the divert target phone. Target Dialog Extension: {0}", targetDialog.Extension);
            }

            targetDialog.RecordVoice = false;
            targetDialog.Status = DialogStatus.Dialing;

            return targetDialog;
        }

        //private List<SipAccount> informingDivertTargets;
        //public List<SipAccount> InformingDivertTargets
        //{
        //    get
        //    {
        //        if (informingDivertTargets == null)
        //        {
        //            var registeredAccounts = Accounts.Where(a => a.Status != SipAccountStatus.Offline && a.MaxConcurrentCalls > 1 && !string.IsNullOrEmpty(a.MatchRule)).ToList();
        //            if (registeredAccounts != null)
        //                informingDivertTargets = registeredAccounts;
        //        }
        //        return informingDivertTargets;
        //    }
        //}

        public SipDialog SoftPhoneDial(string phoneNumber)
        {
            SipDialog dialog = NewDialog(DialogStatus.Dialing, null, string.Empty, phoneNumber);

            dialog.RtpNet = new RtpNet(LocalAddress.Address);
            //dialog.ToAccount = toAccount;

            string message = SendTemplateMessage(Template_SoftphoneInvite, Config.Default.SipProxyEndPoint, SoftPhoneUsername, phoneNumber, dialog.RtpNet.LocalPort, dialog.DialogID);
            dialog.InviteMessage = new SipMessage(message);

            return dialog;
        }

        public void StartFax(SipDialog dialog)
        {
            var faxReInviteMessage = dialog.InviteMessage.Clone() as SipMessage;
            faxReInviteMessage.SdpFields.Clear();
            faxReInviteMessage.ContentType.Value = "application/sdp";
            faxReInviteMessage.CSeq.Number++;
            faxReInviteMessage.Via.Branch = GenerateBranch();
            var result = faxReInviteMessage.Content.Trim();

            result = result.Replace("To:", "XX:");
            result = result.Replace("From:", "To:");
            result = result.Replace("XX:", "From:");

            dialog.Status = DialogStatus.ReInvitingForFax;
            SendTemplateMessage(Template_FaxReinvite, dialog.FromAccount.SipEndPoint, result, dialog.RtpNet.LocalPort, faxReInviteMessage.HeaderFirstLine.RequestHeader.MethodUri.UserID);
            Logger.WriteTodo("Check StartFax");
        }

        public SipDialog Dial(string calleeID)
        {
            SipAccount toAccount = FindToAccount(calleeID);
            if (toAccount != null)
            {
                toAccount.Status = SipAccountStatus.Dialing;
                SipDialog dialog = NewDialog(DialogStatus.Dialing, null, string.Empty, toAccount.UserID);

                dialog.RtpNet = new RtpNet(LocalAddress.Address);
                dialog.ToAccount = toAccount;
                dialog.DialogType = toAccount.MaxConcurrentCalls == 1 ? DialogType.ClientIncomming : DialogType.GatewayOutgoing;

                SendInvite(dialog, string.Empty, calleeID, dialog.RtpNet.LocalPort,
                    dialog.DialogID, Config.Default.SipProxyEndPoint, string.Empty, string.Empty, null);

                return dialog;
            }
            else
                return null;
        }

        internal void PlayVoice(SipDialog dialog, byte[] voice)
        {
            if (voice != null)
            {
                dialog.VoiceStreamOffset = 0;
                dialog.VoiceStream = voice;
            }
        }

        internal void StopPlayVoice(SipDialog dialog)
        {
            dialog.VoiceStreamOffset = -1;
            dialog.VoiceStream = null;
        }

        public void RejectCall(SipDialog dialog)
        {
        }

        public void AnswerCall(SipDialog dialog)
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

            dialog.Status = DialogStatus.InviteWaitForAck;
            Logger.WriteDebug("Dialog {0} answered, wait for ack.", dialog.Call.DialogID);
        }

        public void SoftPhoneAnswerCall(SipDialog dialog)
        {
            string via = string.Join(",", dialog.InviteMessage.Vias.Select(f => f.Value).ToArray());

            SendTemplateMessage(Template_SoftphoneOk, dialog.InviteMessage.Via.EndPoint,
                dialog.RtpNet.LocalPort,
                dialog.DialogID,
                via,
                dialog.InviteMessage.From.Value,
                dialog.InviteMessage.To.Value,
                dialog.InviteMessage.CSeq.Value,
                dialog.InviteMessage.To.Uri.Value,
                Generate32bitRandomNumber(),
                LocalAddress.Address);

            dialog.Status = DialogStatus.InviteWaitForAck;
            Logger.WriteDebug("Dialog {0} answered, wait for ack.", dialog.Call.DialogID);
        }

        internal void StartRecord(string callID)
        {
            Logger.WriteDebug("StartRecordingVoice on callID :{0}", callID);
            var dialog = FindDialog(callID);
            dialog.RecordedVoiceStream = new System.IO.MemoryStream();
            dialog.Status = DialogStatus.Recording;
        }

        internal void StopRecord(string callID)
        {
            Logger.WriteDebug("GetRecordedVoice on callID :{0}", callID);
            var dialog = FindDialog(callID);
            dialog.Status = DialogStatus.Connect;
            //lock (dialog.RecordedVoiceStreamSyncObject)
            //{
            //    dialog.RecordedVoiceStream.Close();
            //}
        }

        public void TransferCall(SipDialog dialog, DivertTarget target, bool isProxy, bool recordVoice)
        {
            Logger.WriteInfo("TransferCall user: '{3}', LastCallTime: {4}, LastCallEndedTime: {5}, RegisterTime: {6} callID:{0}, target: {1}, isProxy:{2}, callerID: {7}",
                dialog.DialogID, target.Phone, isProxy, target.Account.UserID, target.Account.LastCallTime,
                target.Account.LastCallEndedTime, target.Account.RegisterTime, dialog.CallerID);

            dialog.CurrentlyDivertTarget = target;
            if (isProxy)
            {
                target.Account.Status = SipAccountStatus.Dialing;
                SipDialog targetDialog = Dial(dialog.CallerID, target.Phone, target.Account, string.Empty, dialog.Call.GraphTrack);

                if (dialog.DialogType == DialogType.GatewayOutgoing || dialog.DialogType == DialogType.ClientOutgoing ||
                    targetDialog.DialogType == DialogType.GatewayOutgoing || targetDialog.DialogType == DialogType.ClientOutgoing)
                {
                    //In order to generate proper ACK for Cisco during outcall session initiation.
                    //For more information review the comment inside SendAck method.
                    targetDialog.Extension = target.Phone;
                    Logger.WriteImportant("Setting target dialog extension with the divert target phone. Target Dialog Extension: {0}", targetDialog.Extension);
                }

                targetDialog.QueueEnterTime = dialog.QueueEnterTime;

                //targetDialog.TransferFromAccount = dialog.FromAccount;
                targetDialog.RecordVoice = recordVoice;

                dialog.DivertPartner = targetDialog;
                targetDialog.DivertPartner = dialog;
                dialog.Status = DialogStatus.DivertingWaitForTargetResponse;
            }
            else
            {
                dialog.Status = DialogStatus.ReferingWaitForAccept;

                string referToExtension = string.Empty;
                if (Config.Default.TransferMode == TransferMode.Attended)
                    referToExtension = string.Format("?Replaces=callid:{0};to-tag={2};from-tag={1}", dialog.DialogID, dialog.InviteMessage.To.Tag, dialog.InviteMessage.From.Tag);

                SendTemplateMessage(Template_Refer, dialog.FromAccount.SipEndPoint,
                    dialog.FromAccount.UserID,
                    target.Phone,
                    dialog.DialogID,
                    dialog.InviteMessage.To.Value,
                    dialog.InviteMessage.From.Value,
                    dialog.InviteMessage.To.Tag,
                    dialog.InviteMessage.HeaderFirstLine.RequestHeader.MethodUri,
                    referToExtension,
                    dialog.InviteMessage.From.UriWithDisplayName);
            }
        }

        public void TransferCall(SipDialog dialog, DivertTarget target, bool isProxy, bool recordVoice, out SipDialog targetDialog)
        {
            Logger.WriteInfo("TransferCall user: '{3}', LastCallTime: {4}, LastCallEndedTime: {5}, RegisterTime: {6} callID:{0}, target: {1}, isProxy:{2}, callerID: {7}",
                dialog.DialogID, target.Phone, isProxy, target.Account.UserID, target.Account.LastCallTime,
                target.Account.LastCallEndedTime, target.Account.RegisterTime, dialog.CallerID);

            targetDialog = null;

            dialog.CurrentlyDivertTarget = target;
            if (isProxy)
            {
                target.Account.Status = SipAccountStatus.Dialing;
                //////
                targetDialog = Dial(dialog.CallerID, target.Phone, target.Account, string.Empty, dialog.Call.GraphTrack);

                if (dialog.DialogType == DialogType.GatewayOutgoing || dialog.DialogType == DialogType.ClientOutgoing ||
                    targetDialog.DialogType == DialogType.GatewayOutgoing || targetDialog.DialogType == DialogType.ClientOutgoing)
                {
                    //In order to generate proper ACK for Cisco during outcall session initiation.
                    //For more information review the comment inside SendAck method.
                    targetDialog.Extension = target.Phone;
                    Logger.WriteImportant("Setting target dialog extension with the divert target phone. Target Dialog Extension: {0}", targetDialog.Extension);
                }

                targetDialog.QueueEnterTime = dialog.QueueEnterTime;

                //targetDialog.TransferFromAccount = dialog.FromAccount;
                targetDialog.RecordVoice = recordVoice;

                dialog.DivertPartner = targetDialog;
                targetDialog.DivertPartner = dialog;
                dialog.Status = DialogStatus.DivertingWaitForTargetResponse;
            }
            else
            {
                dialog.Status = DialogStatus.ReferingWaitForAccept;

                string referToExtension = string.Empty;
                if (Config.Default.TransferMode == TransferMode.Attended)
                    referToExtension = string.Format("?Replaces=callid:{0};to-tag={2};from-tag={1}", dialog.DialogID, dialog.InviteMessage.To.Tag, dialog.InviteMessage.From.Tag);

                SendTemplateMessage(Template_Refer, dialog.FromAccount.SipEndPoint,
                    dialog.FromAccount.UserID,
                    target.Phone,
                    dialog.DialogID,
                    dialog.InviteMessage.To.Value,
                    dialog.InviteMessage.From.Value,
                    dialog.InviteMessage.To.Tag,
                    dialog.InviteMessage.HeaderFirstLine.RequestHeader.MethodUri,
                    referToExtension,
                    dialog.InviteMessage.From.UriWithDisplayName);
            }
        }

        public void TransferCallToEavesdropper(SipDialog firstLegDialog, SipAccount targetAccount, SipDialog auditDialog)
        {
            System.Threading.Thread.Sleep(400);

            DivertTarget target = new DivertTarget(targetAccount, targetAccount.UserID);

            target.Account.Status = SipAccountStatus.Dialing;
            SipDialog targetDialog = Dial(firstLegDialog.CallerID, target.Phone, target.Account, string.Empty, string.Empty);

            targetDialog.CurrentlyDivertTarget = target;

            targetDialog.RecordVoice = false;
            targetDialog.Status = DialogStatus.Dialing;
            targetDialog.Extension = "ed";

            string operatorID = targetAccount.UserID;
            if (auditDialog != null)
            {
                Logger.WriteInfo("StartAudition, OperatorId: {0}, Audit Dialog Id: {1}, Eavesdropper Dialog Id: {2}", operatorID, auditDialog.DialogID, targetDialog.DialogID);

                auditDialog.AuditionEnabled = true;
                auditDialog.AuditionTarget = targetDialog;

                targetDialog.Sequence = (short)new Random().Next(0, 255);
                targetDialog.TimeStamp = (int)DateTime.Now.Ticks;
                targetDialog.SSRC = new Random().Next(0, int.MaxValue);
            }
            else
                Logger.WriteInfo("StartAudition, operaotrID: {0} not found.", operatorID);
        }

        internal void Hold(string dialogID)
        {
            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == dialogID);
            if (dialog == null)
            {
                Logger.WriteWarning("Hold Request by client on not found dialog: {0}", dialogID);
                return;
            }
            Logger.WriteDebug("Hold Request by client dialog: {0}", dialogID);

            dialog.Status = DialogStatus.Hold;
            if (dialog.DivertPartner != null)
                dialog.DivertPartner.Status = DialogStatus.Hold;
        }

        internal void UnHold(string dialogID)
        {
            var dialog = Dialogs.FirstOrDefault(d => d.DialogID == dialogID);
            if (dialog == null)
            {
                Logger.WriteWarning("UnHold Request by client on not found dialog: {0}", dialogID);
                return;
            }

            if (dialog.Status == DialogStatus.Hold)
                Logger.WriteDebug("UnHold Request by client dialog: {0}", dialogID);
            else
                Logger.WriteWarning("UnHold Request by client dialog: {0}, status is not Hold: {1}", dialogID, dialog.Status);

            dialog.Status = DialogStatus.Talking;
            if (dialog.DivertPartner != null)
                dialog.DivertPartner.Status = DialogStatus.Talking;
        }

        internal void RejectCall(string callID)
        {
            Logger.WriteInfo("RejectCall callID:{0}", callID);
            var dialog = FindDialog(callID);

            dialog.InviteMessage.PopVia(dialog.ForwardInviteViaBranch);

            SendTemplateMessage(Template_BusyHere, dialog.FromAccount.SipEndPoint, dialog.InviteMessage.Via.Value, dialog.InviteMessage.To.Value, dialog.InviteMessage.From.Value, callID);

            dialog.Status = DialogStatus.RejectWaitForAck;
        }
    }
}

