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

namespace UMSV
{
    public class UmsvSoftPhone : SoftPhone
    {
        #region Fields

        System.Threading.Timer SendRegisterPacketTimer;
        SipService sipService = new SipService();
        public SipDialog CurrentDialog;
        DirectSoundOut waveOut;
        BufferedWaveProvider waveProvider;
        WaveIn waveIn;
        bool Recording = false;

        int WelcomeAnnounceOffset = 0;
        Multimedia.Timer PlayWelcomeAnnounceTimer;

        #endregion

        public override bool Start(string username, string password)
        {
            base.Start(username, password);

            try
            {
                SipService.IsSoftPhoneMode = true;
                sipService.Start(LocalAddress);
                sipService.SoftPhoneUsername = username;
                sipService.SoftPhonePassword = password;

                waveIn = new WaveIn();
                var recordStream = new MemoryStream();
                var waveStream = new RawSourceWaveStream(recordStream, AudioUtility.PcmFormat);
                waveIn.BufferMilliseconds = 20;
                waveIn.DataAvailable += new EventHandler<WaveInEventArgs>(waveIn_DataAvailable);

                sipService.IncommingCall += new EventHandler<IncommingCallEventArgs>(SipService_OnIncommingCall);
                sipService.CallStablished += new EventHandler<CallStablishedEventArgs>(SipService_OnCallStablished);
                sipService.CallDisconnected += new EventHandler<CallDisconnectedEventArgs>(SipService_OnCallDisconnected);
                sipService.Registered += SipService_Registered;
                sipService.UnRegistered += SipService_UnRegistered;

                SendRegisterPacketTimer = new System.Threading.Timer(SendRegisterPacket, null, 0, Config.Default.ClientRegisterPeriod);
                PlayWelcomeAnnounceTimer = new Multimedia.Timer();
                PlayWelcomeAnnounceTimer.Tick += new EventHandler(PlayWelcomeAnnounce);
                PlayWelcomeAnnounceTimer.Period = SipService.RtpInterval;

                waveOut = new DirectSoundOut();
                waveProvider = new BufferedWaveProvider(Folder.Audio.AudioUtility.PcmFormat);
                waveOut.Init(waveProvider);
                waveOut.Play();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
                return false;
            }
        }

        public override void SendDtmf(char key)
        {
            throw new NotImplementedException();
        }

        void StopRecording()
        {
            try
            {
                if (Recording)
                {
                    Logger.WriteInfo("StopRecording...");
                    waveIn.StopRecording();
                    Recording = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public override void DisconnectCall()
        {
            AccountStatus = SipAccountStatus.Idle;
            StopRecording();

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Visibility = Visibility.Hidden;
            }


            Logger.WriteDebug("PortSip.Core.terminateCall, CallID:{0}", CurrentDialog.Call.DialogID);
            sipService.SoftPhoneDisconnectCall(CurrentDialog);
            OnCallDisconnected();
            Folder.Console.ShowStatusMessage("تماس قطع شد");
        }

        public override void Stop()
        {
            base.Stop();
            sipService.Stop();
        }

        public override void UnRegister()
        {
            if (SendRegisterPacketTimer != null)
            {
                SendRegisterPacketTimer.Dispose();
                SendRegisterPacketTimer = null;
            }

            if (AccountStatus == SipAccountStatus.Offline)
                return;

            Logger.WriteInfo("UnRegistering ...");
            sipService.UnRegister(string.Empty);
            CallAlertWindow.HideCallAlert();
            IncommingCallPlayer.Stop();
        }

        public override void AnswerCall()
        {
            CallFailureCounter = 0;

            if (CurrentDialog.Call != null)
                Logger.WriteInfo("AnswerCall DialogID:{0}", CurrentDialog.Call.DialogID);
            else
                Logger.WriteError("AnswerCall CallerID:{0}, Call is null", CurrentDialog.CallerID);

            if (AccountStatus != SipAccountStatus.Dialing)
            {
                Logger.WriteInfo("Invalid AnswerCall, current state:{0}", AccountStatus);
                return;
            }

            sipService.SoftPhoneAnswerCall(CurrentDialog);

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Dispatcher.Invoke((Action)(() =>
                    {
                        Global.Default.TalkingImage.Visibility = Visibility.Visible;
                    }));
            }
        }

        public override void RejectCall()
        {
            sipService.RejectCall(CurrentDialog);
            IncommingCallPlayer.Stop();
            CallAlertWindow.HideCallAlert();
            StopRecording();
            AccountStatus = SipAccountStatus.Idle;
        }

        public override bool Call(string target)
        {
            CurrentDialog = sipService.SoftPhoneDial(target);
            return true;
        }

        void SipService_UnRegistered(object sender, UnRegisteredEventArgs e)
        {
            AccountStatus = SipAccountStatus.Offline;
            Folder.Console.ShowStatusMessage("سیستم آفلاین شد، عدم دریافت تماس جدید");
            OnUnRegistered();
        }

        void SipService_Registered(object sender, RegisteredEventArgs e)
        {
            AccountStatus = AccountStatus == SipAccountStatus.Offline ? SipAccountStatus.Idle : AccountStatus;
            OnRegistered();
        }

        void SipService_OnCallDisconnected(object sender, CallDisconnectedEventArgs e)
        {
            Logger.WriteInfo("SipService_OnCallDisconnected");
            StopRecording();

            if (Global.Default != null)
            {
                if (Global.Default.TalkingImage != null)
                    Global.Default.TalkingImage.Dispatcher.Invoke((Action)(() =>
                    {
                        Global.Default.TalkingImage.Visibility = Visibility.Hidden;
                    }));
            }

            OnCallDisconnected();
            CallFailureCounter++;
            CallAlertWindow.HideCallAlert();
            IncommingCallPlayer.Stop();
            CheckCallFailureCounter();
            Folder.Console.ShowStatusMessage("تماس قطع شد");

            //if (Exiting)
            //    Folder.Console.LogOff();
        }

        void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (AccountStatus == SipAccountStatus.Talking)
            {
                // Logger.WriteView("waveIn_DataAvailable");
                var buffer = AudioUtility.ConvertRawPcmToRawAlaw(e.Buffer);
                sipService.SendVoiceChunkStream(CurrentDialog, buffer);
            }
        }

        void SipService_OnCallStablished(object sender, CallStablishedEventArgs e)
        {
            Logger.WriteInfo("SipService_OnCallStablished");

            if (Config.Default.AnnounceOperatorUserIDOnAnswer)
            {
                AccountStatus = SipAccountStatus.Hold;
                WelcomeAnnounceOffset = 0;
                PlayWelcomeAnnounceTimer.Start();
            }
            else
            {
                AccountStatus = SipAccountStatus.Talking;
                StopRecording();
                StartRecording();
            }

            OnCallAnswered();
            Folder.Console.ShowStatusMessage("برقراری ارتباط با مشترک");
        }

        void StartRecording()
        {
            try
            {
                if (!Recording)
                {
                    Logger.WriteInfo("StartRecording...");
                    waveIn.StartRecording();
                    Recording = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        void SipService_OnIncommingCall(object sender, IncommingCallEventArgs e)
        {
            AccountStatus = SipAccountStatus.Dialing;
            CallAlertWindow.ShowCallAlert();
            CurrentDialog = e.Dialog;
            DialogID = e.Dialog.Call.DialogID;
            CallerID = e.Dialog.Call.CallerID;

            Logger.WriteInfo("SipService_OnIncommingCall, Receiving RTP ...");
            e.Dialog.RtpNet.OnReceive += new PacketTransmitEventHandler(RtpNet_OnReceive);
            StopRecording();
            OnIncommingCall(e);
        }

        void RtpNet_OnReceive(RtpNet sender, byte[] packet)
        {
            if (AccountStatus == SipAccountStatus.Talking)
            {
                //Logger.WriteView("RtpNet_OnReceive");
                var buffer = AudioUtility.ConvertRawAlawToRawPcm(packet.Skip(12).ToArray());
                waveProvider.AddSamples(buffer, 0, buffer.Length);
            }
            //else
            //    Logger.WriteView("RtpNet_OnReceive NOT SipAccountStatus.Talking");
        }

        void PlayWelcomeAnnounce(object sender, EventArgs e)
        {
            try
            {
                if (WelcomeAnnounceOffset > WelcomeAnnounce.Length - SipService.RtpChunkSize)
                {
                    PlayWelcomeAnnounceFinished();
                }
                else
                {
                    sipService.SendVoiceChunkStream(CurrentDialog, WelcomeAnnounce.Skip(WelcomeAnnounceOffset).Take(SipService.RtpChunkSize).ToArray());
                    WelcomeAnnounceOffset += SipService.RtpChunkSize;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "PlayWelcomeAnnounce");
            }
        }

        void PlayWelcomeAnnounceFinished()
        {
            Logger.WriteView("PlayWelcomeAnnounceFinished");
            PlayWelcomeAnnounceTimer.Stop();
            CallAlertWindow.HideCallAlert();
            IncommingCallPlayer.Stop();
            AccountStatus = SipAccountStatus.Talking;
            StopRecording();
            StartRecording();
        }

        void SendRegisterPacket(object state)
        {
            sipService.Register(string.Empty);
        }

        public override void Transfer(string number)
        {
            SipAccount account = null;
            DivertTarget target = new DivertTarget(account, number);
            sipService.TransferCall(CurrentDialog, target, true, false);
        }

        byte[] WelcomeAnnounce;
        protected override void CreateOperatorUserIDAnnounceOnAnswer(string userID)
        {
            if (Config.Default.AnnounceOperatorUserIDOnAnswer)
            {
                try
                {
                    MemoryStream stream = new MemoryStream();

                    Logger.WriteView("Creating Welcome announce for user :{0}", userID);
                    string userVoiceName = string.Format(Config.Default.OperatorIncommingCallWelcomeVoiceFormat, userID, User.Current.ID);

                    byte[] operatorVoice = UMSV.Voice.GetByName(userVoiceName);
                    if (operatorVoice != null)
                        stream.Write(operatorVoice, 0, operatorVoice.Length);

                    byte[] befarmaeidVoice = UMSV.Voice.GetByName(Constants.Voice_Befarmaeid);
                    if (befarmaeidVoice != null)
                        stream.Write(befarmaeidVoice, 0, befarmaeidVoice.Length);

                    WelcomeAnnounce = stream.ToArray();
                }
                catch (Exception ex)
                {
                    Logger.Write(ex);
                }
            }
        }
    }
}
