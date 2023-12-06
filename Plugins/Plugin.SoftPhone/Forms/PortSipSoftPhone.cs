using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMSV;
using System.Runtime.InteropServices;
using Enterprise;
using System.Media;
using System.Windows.Forms;
using Folder.Audio;
using System.Net;
using UMSV.Schema;
using System.Windows.Input;
using System.Windows;
using Folder;
using System.Xml.Linq;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NAudio.Wave;
using System.Windows.Forms.Integration;
using AxPortSIPCoreLibLib;
using System.Text.RegularExpressions;

namespace UMSV
{
    public class PortSipSoftPhone : SoftPhone
    {
        bool Stopping = false;
        PortSipAdapter PortSip;
        const int AudioCodec_PCMA = 8;
        const int RegisterPeriod = 3600;
        int CurrentSessionId;
        string OperatorUserIDAnnounceFile;
        int OperatorUserIDOnEndAnnounceVoiceLength;

        public override bool Start(string account, string password)
        {
            try
            {
                base.Start(account, password);

                PortSip = new PortSipAdapter();
                var host = new WindowsFormsHost()
                {
                    Child = PortSip
                };

                PortSip.Core.registerSuccess += Core_registerSuccess;
                PortSip.Core.registerFailure += Core_registerFailure;
                PortSip.Core.inviteAnswered += Core_inviteAnswered;
                PortSip.Core.inviteClosed += Core_inviteClosed;
                PortSip.Core.inviteFailure += Core_inviteFailure;
                PortSip.Core.inviteIncoming += Core_inviteIncoming;
                PortSip.Core.arrivedSignaling += Core_arrivedSignaling;
                PortSip.Core.transferTrying += Core_transferTrying;
                PortSip.Core.transferRinging += Core_transferRinging;
                PortSip.Core.ACTVTransferSuccess += Core_ACTVTransferSuccess;
                PortSip.Core.ACTVTransferFailure += Core_ACTVTransferFailure;
                PortSip.Core.PASVTransferSuccess += Core_PASVTransferSuccess;
                PortSip.Core.PASVTransferFailure += Core_PASVTransferFailure;

                if (!Register(account, password))
                {
                    Core_registerFailure(this, null);
                    return false;
                }

                
                //PortSip.Core.enableLog(1);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        void Core_PASVTransferFailure(object sender, _DPortSIPCoreLibEvents_PASVTransferFailureEvent e)
        {
            Logger.WriteDebug("Core_PASVTransferFailure");
            Folder.Console.ShowStatusMessage("انتقال مکالمه انجام نشد.");
        }

        void Core_PASVTransferSuccess(object sender, _DPortSIPCoreLibEvents_PASVTransferSuccessEvent e)
        {
            Logger.WriteDebug("Core_PASVTransferSuccess");
            Folder.Console.ShowStatusMessage("انتقال مکالمه انجام شد.");
        }

        void Core_transferRinging(object sender, _DPortSIPCoreLibEvents_transferRingingEvent e)
        {
            Logger.WriteDebug("Core_transferRinging");
            Folder.Console.ShowStatusMessage("انتقال مکالمه ...");
        }

        void Core_transferTrying(object sender, _DPortSIPCoreLibEvents_transferTryingEvent e)
        {
            Logger.WriteDebug("Core_transferTrying");
            Folder.Console.ShowStatusMessage("انتقال مکالمه ...");
        }

        void Core_ACTVTransferFailure(object sender, _DPortSIPCoreLibEvents_ACTVTransferFailureEvent e)
        {
            Logger.WriteInfo("Core_ACTVTransferFailure");
            OnTransferFailed();
            Folder.Console.ShowStatusMessage("انتقال مکالمه انجام نشد.");
        }

        void Core_ACTVTransferSuccess(object sender, _DPortSIPCoreLibEvents_ACTVTransferSuccessEvent e)
        {
            Logger.WriteInfo("Core_ACTVTransferSuccess");

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Hidden;
            }

            OnCallDisconnected();

            if (Stopping)
                CompleteUnRegister();
            else
                AccountStatus = SipAccountStatus.Idle;

            Folder.Console.ShowStatusMessage("انتقال مکالمه انجام شد.");
        }

        void Core_arrivedSignaling(object sender, _DPortSIPCoreLibEvents_arrivedSignalingEvent e)
        {
            Logger.WriteView(e.signaling);
            OnMessageArrived(e.signaling);
            //currentIncomming.DialogID = Regex.Match(e.signaling, "Call-ID: (?<CallID>.+)", RegexOptions.Singleline).Groups["CallID"].Value;
            //currentIncomming.GraphTrack = Regex.Match(e.signaling, "Graph-Track: (?<GraphTrack>.+)", RegexOptions.Singleline).Groups["CallID"].Value;
        }

        public override void DndStart()
        {
            base.DndStart();
            string to = User.Current.Username, sdp = "s=DND:on";
            PortSip.Core.sendPagerMessage(ref to, ref sdp);

            Logger.WriteDebug("DndStart");
        }

        public override void DndEnd()
        {
            base.DndEnd();
            string to = User.Current.Username, sdp = "s=DND:off";
            PortSip.Core.sendPagerMessage(ref to, ref sdp);

            Logger.WriteDebug("DndEnd");
        }

        void RejectCall(int sessionId, StatusCode statusCode, string callerID)
        {
            Logger.WriteWarning("Incomming call on status: {1}. caller:{0}, sending {2} response.", callerID, AccountStatus, statusCode);
            string reason = statusCode.ToString();
            PortSip.Core.rejectCall(sessionId, (int)statusCode, ref reason);

            Folder.Console.ShowStatusMessage("{0} تماس بدون پاسخ متوالی", CallFailureCounter);
        }

        void Core_inviteIncoming(object sender, _DPortSIPCoreLibEvents_inviteIncomingEvent e)
        {
            Logger.WriteInfo("Core_inviteIncoming sessionId:{0} caller:{1} callee:{2} calleeDisplayName:{3} callerDisplayName:{4}", e.sessionId, e.caller, e.callee, e.calleeDisplayName, e.callerDisplayName);

            if (Stopping)
            {
                Logger.WriteWarning("Incomming call while stopping, rejecting call...");
                RejectCall(e.sessionId, StatusCode.ServiceUnavailable, e.caller);
                return;
            }
            else if (AccountStatus != SipAccountStatus.Idle)
            {
                switch (AccountStatus)
                {
                    case SipAccountStatus.Dialing:
                    case SipAccountStatus.Hold:
                    case SipAccountStatus.Talking:
                        RejectCall(e.sessionId, StatusCode.TemporarilyUnavailable, e.caller);
                        break;

                    case SipAccountStatus.DND:
                        RejectCall(e.sessionId, StatusCode.DoNotDisturb/*(StatusCode)Config.Default.StatusCodeOnDndRejection*/, e.caller);
                        break;

                    case SipAccountStatus.Offline:
                        RejectCall(e.sessionId, StatusCode.ServiceUnavailable, e.caller);
                        break;
                }
                return;
            }
            else if (DoNotDisturb)
            {
                RejectCall(e.sessionId, StatusCode.DoNotDisturb/*(StatusCode)Config.Default.StatusCodeOnDndRejection*/, e.caller);
                return;
            }

            CurrentSessionId = e.sessionId;
            AccountStatus = SipAccountStatus.Dialing;
            CallAlertWindow.ShowCallAlert();

            SipDialog dialog = new SipDialog();
            dialog.Call.DialogID = e.calleeDisplayName;
            dialog.Call.CallerID = e.callerDisplayName;
            DialogID = e.calleeDisplayName;
            CallerID = e.callerDisplayName;
            OnIncommingCall(new IncommingCallEventArgs(dialog));
        }

        void Core_inviteFailure(object sender, _DPortSIPCoreLibEvents_inviteFailureEvent e)
        {
            Logger.WriteInfo("Core_inviteFailure, reason: {0}", e.reason);
            CallAlertWindow.HideCallAlert();

            IncommingCallPlayer.Stop();

            if (Stopping)
                CompleteUnRegister();
            else
                AccountStatus = SipAccountStatus.Idle;

            //Folder.Console.ShowStatusMessage("تماس قطع شد");
            Folder.Console.ShowStatusMessage("{0} تماس بدون پاسخ متوالی", CallFailureCounter);
        }

        void Core_inviteClosed(object sender, _DPortSIPCoreLibEvents_inviteClosedEvent e)
        {
            Logger.WriteInfo("Core_inviteClosed");
            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Hidden;
            }

            IncommingCallPlayer.Stop();

            OnCallDisconnected();

            if (AccountStatus != SipAccountStatus.Talking)
                CallFailureCounter++;

            CallAlertWindow.HideCallAlert();
            CheckCallFailureCounter();
            Logger.WriteInfo("Core_inviteClosed -> CallFailureCounter:{0} ", CallFailureCounter);

            if (Stopping)
                CompleteUnRegister();
            else
                AccountStatus = SipAccountStatus.Idle;

            Folder.Console.ShowStatusMessage("{0} تماس بدون پاسخ متوالی", CallFailureCounter);

        }

        void Core_inviteAnswered(object sender, _DPortSIPCoreLibEvents_inviteAnsweredEvent e)
        {
            CallFailureCounter = 0;

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Visible;
            }

            //IncommingCallPlayer.Stop();
            OnCallAnswered();
        }

        void Core_registerFailure(object sender, _DPortSIPCoreLibEvents_registerFailureEvent e)
        {
            Logger.WriteInfo("Core_registerFailure");

            try
            {
                PortSip.Core.unInitialize();
                int ret = PortSip.Core.registerServer(RegisterPeriod);
                if (ret == 0)
                {
                    Folder.Console.ShowStatusMessage("اختلال در شبکه، تلفن مجدد آنلاین شد.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }

            OnUnRegistered();
            Folder.Console.ShowStatusMessage("تلفن آنلاین نشد، عدم دریافت تماس جدید");
            Folder.MessageBox.ShowError("با عرض پوزش کاربر شما آنلاین نشد، لطفا دوباره سعی کنید!");
        }

        void Core_registerSuccess(object sender, EventArgs e)
        {
            Logger.WriteInfo("Core_registerSuccess");
            OnRegistered();

            CallFailureCounter = 0;

            if (AccountStatus == SipAccountStatus.Offline)
                AccountStatus = SipAccountStatus.Idle;

            Folder.Console.ShowStatusMessage("تلفن آنلاین شد");
        }

        bool Register(string account, string password)
        {
            try
            {
                PortSip.Device.initialize();

                string agent = "PendarSip";
                string userDomain = string.Empty;
                string outbandServer = string.Empty;
                string stunServer = string.Empty;
                string proxyServer = string.Empty;
                try
                {
                    proxyServer = string.IsNullOrWhiteSpace(Config.Default.SoftPhoneRegistrationDomain) ? Config.Default.SipProxyAddress : Config.Default.SoftPhoneRegistrationDomain;
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                    Logger.WriteImportant("Proxy server could not be extracted. Probably config schema is old and does not contain a configuration for softphone registration domain.");
                }
                int proxyPort = Config.Default.SipProxyPort;

                PortSip.Core.setRtpPortRange(Config.Default.RtpLocalPortFrom, Config.Default.RtpLocalPortTo, 0, 0);

                Logger.WriteInfo("Register User:{0} Server:({1}:{2})", account, proxyServer, proxyPort);
                var localIP = LocalAddress.Address.ToString();
                int result = PortSip.Core.initialize(
                    0,
                    2, // Config.Default.SoftPhoneMaxConcurrentCalls
                    ref account,
                    ref account,
                    ref account,
                    ref password,
                    ref agent,
                    ref localIP,
                    LocalAddress.Port,
                    ref userDomain,
                    ref proxyServer,
                    proxyPort,
                    ref outbandServer,
                    36000,
                    ref stunServer,
                    0);

                if (result == -1)
                {
                    Logger.WriteError("Login Failed on PortSip.Core.initialize!");
                    return false;
                }

                PortSip.Core.setRTPJitterBufferLength(2);
                PortSip.Core.clearAudioCodec();
                PortSip.Core.addAudioCodec(AudioCodec_PCMA);

                int ret = PortSip.Core.registerServer(RegisterPeriod);
                if (ret != 0)
                {
                    PortSip.Core.unInitialize();
                    Logger.WriteError("SIP RegistrationFailure");
                    {
                        Logger.WriteError("Login Failed on PortSip.Core.registerServer!");
                        return false;
                    }
                }

                string licenseKey = "2WmTBs6dmS/nVdM35kw7QQ==@FfjO0il3RjDMxkBvLGG8dw==@Dy+WEF5wpaKTGgVpAiAOAQ==@6Xo+rtBKB9Ddf/kWby4Uyw==";
                PortSip.Core.setLicenseKey(ref licenseKey);
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "Softphone.Register");
            }

            return true;
        }

        void CompleteUnRegister()
        {
            Logger.WriteInfo("CompleteUnRegister SoftPhone ...");

            try
            {
                if (AccountStatus == SipAccountStatus.Offline)
                {
                    Logger.WriteWarning("Already unregisterd.");
                    return;
                }

                AccountStatus = SipAccountStatus.Offline;
                CallAlertWindow.HideCallAlert();

                PortSip.Core.registerSuccess -= Core_registerSuccess;
                PortSip.Core.registerFailure -= Core_registerFailure;
                PortSip.Core.inviteAnswered -= Core_inviteAnswered;
                PortSip.Core.inviteClosed -= Core_inviteClosed;
                PortSip.Core.inviteFailure -= Core_inviteFailure;
                PortSip.Core.inviteIncoming -= Core_inviteIncoming;

                PortSip.Core.unInitialize();
                PortSip.Core.unRegisterServer();
                PortSip.Device.unInitialize();

                OnUnRegistered();

                Stopping = false;
                Folder.Console.ShowStatusMessage("سیستم آفلاین شد، عدم دریافت تماس جدید");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public override void UnRegister()
        {
            Stopping = true;

            if (AccountStatus == SipAccountStatus.Idle)
                CompleteUnRegister();
            else
                Folder.Console.ShowStatusMessage("در حال خروج ...");
        }

        public override void AnswerCall()
        {
            CallFailureCounter = 0;

            if (AccountStatus != SipAccountStatus.Dialing)
            {
                Logger.WriteInfo("Invalid AnswerCall, current state:{0}", AccountStatus);
                return;
            }

            Logger.WriteInfo("Answerd sessionId:{0}", CurrentSessionId);

            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Logger.WriteInfo("Answerd in Loop: {0}", i);
                    PortSip.Core.answerCall(CurrentSessionId);
                    
                    break;
                }
                catch (OutOfMemoryException)
                {
                    Logger.WriteError("OutOfMemoryException on Call Answerd.");
                }

                Logger.WriteImportant("i => {0}", i.ToString());
            }



            Logger.WriteDebug("Call Answerd.");
            AccountStatus = SipAccountStatus.Talking;

            Logger.WriteInfo("Befor setPlayWaveFileToRemote");

            if (Config.Default.AnnounceOperatorUserIDOnAnswer)
                PortSip.Core.setPlayWaveFileToRemote(ref OperatorUserIDAnnounceFile, 1, 1);

            

            OnCallAnswered();

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Visible;
            }

            Folder.Console.ShowStatusMessage("برقراری ارتباط با مشترک");
        }

        protected override void CreateOperatorUserIDAnnounceOnAnswer(string userID)
        {
            if (Config.Default.AnnounceOperatorUserIDOnAnswer || Config.Default.AnnounceOperatorUserIDOnEnd)
            {
                try
                {
                    OperatorUserIDAnnounceFile = System.IO.Path.Combine(Folder.SystemFile.FolderCachePath, Constants.OperatorUserIDAnnounceFile + ".wav");

                    MemoryStream stream = new MemoryStream();
                    string userVoiceName = string.Format(Config.Default.OperatorIncommingCallWelcomeVoiceFormat, userID, User.Current.ID);
                    var operatorSpecialVoice = UMSV.Voice.GetByName(userVoiceName);

                    if (operatorSpecialVoice != null)
                    {
                        stream.Write(operatorSpecialVoice, 0, operatorSpecialVoice.Length);
                    }
                    else
                    {
                        var operatorVoice = UMSV.Voice.GetByName(Constants.OperatorUserIDAnnounceFile);
                        stream.Write(operatorVoice, 0, operatorVoice.Length);

                        byte[] operatorNameVoice;

                        if (IsNumber(userID))
                            operatorNameVoice = UMSV.Voice.GetNumberVoice(userID);
                        else
                            operatorNameVoice = UMSV.Voice.GetByName(userID);

                        if (operatorNameVoice != null)
                            stream.Write(operatorNameVoice, 0, operatorNameVoice.Length);
                    }

                    OperatorUserIDOnEndAnnounceVoiceLength = (int)(stream.Position / 8);

                    var befarmaeidVoice = UMSV.Voice.GetByName(Constants.Voice_Befarmaeid);
                    if (befarmaeidVoice != null)
                        stream.Write(befarmaeidVoice, 0, befarmaeidVoice.Length);


                    var rawData = stream.ToArray();
                    OperatorUserIDAnnounceTime = rawData.Length / 8;
                    byte[] waveFile = Folder.Audio.AudioUtility.ConvertRawAlawToRawPcm(rawData);
                    waveFile = Folder.Audio.AudioUtility.AppendPcmHeader(waveFile);
                    System.IO.File.WriteAllBytes(OperatorUserIDAnnounceFile, waveFile);
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }

        private bool IsNumber(string userID)
        {
            foreach (char ch in userID.ToCharArray())
                if (!Char.IsDigit(ch))
                    return false;
            return true;
        }

        public override void Transfer(string number)
        {
            Logger.WriteDebug("Transfering to {0}", number);
            PortSip.Core.refer(CurrentSessionId, ref number);
        }

        public void Dispose()
        {
            UnRegister();
        }

        public override void SendDtmf(char key)
        {
            int keyCode = key == '*' ? 10 : key == '#' ? 11 : key - 48;
            PortSip.Core.sendDtmf(CurrentSessionId, keyCode);
        }

        public override void RejectCall()
        {
            Logger.WriteInfo("RejectCall -> CallFailureCounter:{0} ", CallFailureCounter);
            string reason = "Busy Here";
            PortSip.Core.rejectCall(CurrentSessionId, 486, ref reason);
            CallAlertWindow.HideCallAlert();
            if (Stopping)
                CompleteUnRegister();
            else
                AccountStatus = SipAccountStatus.Idle;

            Folder.Console.ShowStatusMessage("{0} تماس بدون پاسخ متوالی", CallFailureCounter);
        }

        public override bool Call(string target)
        {
            target = target.Replace(" ", "");
            if (target == string.Empty)
                return false;

            var privilegedDialTargets = User.Current[Constants.UserProfileKey_PrivilegedDialTargets];
            if (!string.IsNullOrWhiteSpace(privilegedDialTargets))
            {
                if (!Regex.IsMatch(target, privilegedDialTargets))
                {
                    Logger.WriteDebug("cannot dial number '{0}' because of pattern '{1}' ...", target, privilegedDialTargets);
                    Folder.Console.ShowStatusMessage("تماس با شماره تلفن '{0}' مجاز نمیباشد.", target);
                    return false;
                }
            }

            Logger.WriteDebug("Calling phone number '{0}' ...", target);
            CurrentSessionId = PortSip.Core.call(ref target, 1);
            AccountStatus = SipAccountStatus.Dialing;
            return true;
        }

        public override void DisconnectCall()
        {
            Logger.WriteDebug("DisconnectCall {0}", CurrentSessionId);
            AccountStatus = SipAccountStatus.Idle;

            //if (IsHolded)
            //    UnHold();

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Hidden;
            }

            Logger.WriteDebug("PortSip.Core.terminateCall, sessionID:{0}", CurrentSessionId);
            if (PortSip != null)
            {
                if (Config.Default.AnnounceOperatorUserIDOnEnd)
                {
                    PortSip.Core.setPlayWaveFileToRemote(ref OperatorUserIDAnnounceFile, 1, 1);
                    System.Threading.Thread.Sleep(OperatorUserIDOnEndAnnounceVoiceLength);
                }

                PortSip.Core.terminateCall(CurrentSessionId);
            }
            OnCallDisconnected();
            Folder.Console.ShowStatusMessage("تماس قطع شد");

            ClearCall();
            if (Stopping)
                CompleteUnRegister();
        }

        //simple
        public override void Hold()
        {
            int res = PortSip.Core.hold(CurrentSessionId);

            if (IsHolded)
                return;

            IsHolded = true;
            //VoipServiceClient.Default.Hold(CurrentDialogID);

            Folder.Console.ShowStatusMessage("تماس در حالت Hold");

            var serverTime = new FolderDataContext().GetDate().Value;

            using (UmsDataContext dc = new UmsDataContext())
            {
                var record = new Session()
                {
                    StartTime = serverTime,
                    EndTime = serverTime,
                    UserID = User.Current.ID,
                    SipID = User.Current.Username,
                    MachineAddress = BitConverter.ToInt32(LocalAddress.Address.GetAddressBytes(), 0),
                    Type = (int)ClientEventType.Hold,
                };
                dc.Sessions.InsertOnSubmit(record);
                dc.SubmitChanges();
                HoldEventRecord = record.ID;
                Logger.WriteInfo("Hold record saved: {0}", HoldEventRecord);
            }
        }



        //simple
        public override void UnHold()
        {
            int res = PortSip.Core.unHold(CurrentSessionId);

            if (!IsHolded)
                return;

            IsHolded = false;
            //VoipServiceClient.Default.UnHold(CurrentDialogID);
            Folder.Console.ShowStatusMessage("برقراری مکالمه ...");

            if (HoldEventRecord.HasValue)
                SaveUnHold();
            else
                Logger.WriteException("Hold record not set before on unhold!");
        }

    }

}
