using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using UMSV.Schema;
using Enterprise;
using Folder;
using System.Data.SqlClient;
using System.Net;
using System.Diagnostics;

namespace UMSV
{
    public partial class SipDialog : ISipDialog
    {
        #region Constructors

        public SipDialog()
        {
            Call = new Call();
        }

        public SipDialog(DialogStatus status, string callID, string callerID, string calleeID, IPAddress localAddress)
        {
            Call = new Call()
            {
                CallTime = DateTime.Now,
                DialogID = string.IsNullOrEmpty(callID) ? Guid.NewGuid().ToString() + "@" + Config.Default.SipProxyEndPoint.Address : callID,
                CallerID = callerID,
                CalleeID = calleeID,
                GraphTrack = string.Empty,
            };

            Sequence = (short)new Random().Next(0, 255);
            TimeStamp = (int)DateTime.Now.Ticks;
            SSRC = new Random().Next(0, int.MaxValue);
            VoiceStreamOffset = -1;
            DtmfDetecting = false;
            RtpNet = new RtpNet(localAddress);

            this.Status = status;
        }

        #endregion

        #region Graph

        [XmlIgnore]
        public UMSV.Schema.Graph Graph;

        [XmlIgnore]
        public Node CurrentNode;

        [XmlIgnore]
        public string CurrentNodeID
        {
            get
            {
                if (CurrentNode == null)
                    return null;
                else
                    return CurrentNode.ID;
            }
            set
            {
                CurrentNode = Graph.FindNodeById(value);
            }
        }

        [XmlIgnore]
        public IGraphAddin GraphAddins;

        #endregion

        #region Methods

        internal void Disconnected(DisconnectCause disconnectCause)
        {
            Logger.WriteView("Dialog {0} Disconnected, cause: {1}", DialogID, disconnectCause);

            Status = DialogStatus.Disconnected;

            //Change status of the operators to idle
            if (ToAccount != null)
                ToAccount.DialogDisconnected(DialogID);

            if (FromAccount != null)
                FromAccount.DialogDisconnected(DialogID);
            ///////////////

            if (!SipService.IsSoftPhoneMode)
                SaveCall(disconnectCause);

            if (DivertPartner != null && (DialogType)Call.Type == DialogType.ClientIncomming)
            {
                if (DivertPartner.DivertPartner == this)
                    DivertPartner.DivertPartner = null;
                DivertPartner = null;
            }

            if (RtpNet != null)
                RtpNet.Stop();
        }

        internal void CheckForStatusTimeout()
        {
            try
            {
                var timeout = Config.Default.Timeouts.FirstOrDefault(t => t.DialogStatus == Status);
                if ((timeout != null && DateTime.Now.Subtract(StatusChangeTime).TotalMilliseconds > timeout.Value))
                {
                    Logger.WriteDebug("Message timeouted on status {0}, callID:{1}", Status, DialogID);
                    if (StatusTimeout != null)
                        StatusTimeout(this, new StatusTimeoutEventArgs());
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        void SaveCall(DisconnectCause disconnectCause)
        {
            try
            {
                if (Call.ID != Guid.Empty) // already saved
                    return;

                if (DateTime.Now.Subtract(Call.CallTime).TotalMilliseconds < Config.Default.TalkingMinimumValidTimeToSave)
                {
                    Logger.WriteWarning("SaveCall '{0}': too small dialog call length for saving, calltime: {1}, disconnectCause: {2}, calleeID: {3}, callerID: {4}",
                        DialogID, CallTime, disconnectCause, CalleeID, CallerID);
                    return;
                }

                Call.ID = Guid.NewGuid();
                Call.DisconnectTime = DateTime.Now;
                Call.DisconnectCause = (int)disconnectCause;

                using (UmsDataContext dc = new UmsDataContext())
                {
                    dc.Calls.InsertOnSubmit(Call);
                    dc.SubmitChanges();
                }

                SaveTalkingVoice();
            }
            catch (SqlException ex)
            {
                Logger.WriteError("SaveCall '{0}': error dialogID: {0}, {1}->{2} {3} {4}", DialogID, ex.Message, Call.GraphTrack, Call.LastNodeID, Call.LastNodeName);

            }
            catch (System.Data.SqlTypes.SqlTypeException ex)
            {
                Logger.WriteError("SaveCall '{0}': error dialogID: {0}, {1}", DialogID, ex.Message);
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        void SaveTalkingVoice()
        {
            if (RecordVoice)
            {
                //Do not Record direct calls between operators
                if (IsForwardMode)
                {
                    Logger.WriteDebug("SaveCall: No recording for direct calls - callerID:{2}, calleeID:{3}, dialogID:{0}, DialogType:{1}", DialogID, DialogType, CallerID, CalleeID);
                    return;
                }

                var partner = DivertPartner;

                // Save Operator voice in the subscriber dialog when recording of played tel number is enabled. The Record will be saved later 
                // when the telephone number is played or subscriber disconnects. 
                if (DialogType == UMSV.DialogType.ClientIncomming && Config.Default.RecordPlayedTelephoneNumber == true)
                {
                    if (RecordedVoiceStartTime.HasValue && partner != null && partner.RecordedVoiceStartTime.HasValue)
                    {
                        partner.RecordedVoiceStream_Partner = new System.IO.MemoryStream();
                        RecordedVoiceStream.WriteTo(partner.RecordedVoiceStream_Partner);
                        partner.RecordedVoiceStartTime_Partner = RecordedVoiceStartTime;

                        Logger.WriteDebug("Save operator (DialogID:{0}) record in subscriber(DialogID:{1}) dialog. Record Size:{2}, StartTime:{3}", DialogID, partner.DialogID, partner.RecordedVoiceStream_Partner.Length, partner.RecordedVoiceStartTime_Partner);
                    }
                    // Subscriber is disconnected first
                    else if (RecordedVoiceStartTime.HasValue)
                        Logger.WriteDebug("Subscriber is disconnected first. dialog:{0}, DialogType:{1} ", DialogID, DialogType);
                    // Operator rejected the call or subscriber disconnected when operator phone is ringing
                    else
                        Logger.WriteDebug("No voice to record for dialog:{0}, DialogType:{1} ", DialogID, DialogType);

                    //Return to save record later when the client disconnects
                    return;
                }
                // When recording of the played tel number is disabled, only save Subscriber voice in the operator dialog following by saving 
                // the record in file. <(Same as previous version of UMSV)>
                else if ((DialogType == UMSV.DialogType.ClientIncomming && Config.Default.RecordPlayedTelephoneNumber == false) ||
                          (DialogType == UMSV.DialogType.ClientOutgoing))
                {
                    if (RecordedVoiceStartTime.HasValue && partner != null && partner.RecordedVoiceStartTime.HasValue)
                    {
                        RecordedVoiceStream_Partner = new System.IO.MemoryStream();
                        partner.RecordedVoiceStream.WriteTo(RecordedVoiceStream_Partner);
                        RecordedVoiceStartTime_Partner = partner.RecordedVoiceStartTime;
                    }
                    else
                    {
                        Logger.WriteDebug("No voice to record for dialog:{0}, DialogType:{1} ", DialogID, DialogType);
                        return;
                    }
                }
                // When recording of the played telephone number is enabled, we save record when the subscriber is disconnected
                else if (DialogType == UMSV.DialogType.GatewayIncomming && Config.Default.RecordPlayedTelephoneNumber)
                {
                    // The BYE is sent by the operator before - The RecordedVoiceStartTime_Partner is set previously by the operator
                    if (RecordedVoiceStartTime.HasValue && RecordedVoiceStartTime_Partner.HasValue)
                    {
                        Logger.WriteDebug("Saving record for Subscriber with DialogID:{0}, Record size:{1}, StartTime:{2} [Operator is disconnected before: operator Record size:{3}, StartTime:{4}]", DialogID, RecordedVoiceStream.Length, RecordedVoiceStartTime, RecordedVoiceStream_Partner.Length, RecordedVoiceStartTime_Partner);
                    }
                    // Subscriber sends BYE first.
                    else
                    {
                        // Subscriber is connected to the operator
                        if (partner != null && partner.RecordedVoiceStartTime.HasValue)
                        {
                            Logger.WriteDebug("Subscriber with DialogID:{0} sends BYE when he is connected to the operator with DialogID:{1} - Subscriber(Record size:{2}, StartTime:{3}) - Operator(Record size:{4}, StartTime:{5})", DialogID, partner.DialogID, RecordedVoiceStream.Length, RecordedVoiceStartTime, partner.RecordedVoiceStream.Length, partner.RecordedVoiceStartTime);
                            RecordedVoiceStream_Partner = new System.IO.MemoryStream();
                            partner.RecordedVoiceStream.WriteTo(RecordedVoiceStream_Partner);
                            RecordedVoiceStartTime_Partner = partner.RecordedVoiceStartTime;
                        }
                        // Type1: Operator phone is ringing and subscriber sends bye
                        // Type2: Subscriber is not connected to an operator yet but sends bye (Subscriber is in queue)
                        else
                        {
                            Logger.WriteDebug("No voice to record for dialog:{0}, DialogType:{1} ", DialogID, DialogType);
                            return;
                        }
                    }
                }
                else
                {
                    Logger.WriteDebug("SaveCall '{0}': Voice will not saved because its type is {1}", DialogID, DialogType);
                    return;
                }

                try
                {
                    // Save voice file
                    VoiceFileManager.SaveVoice(Call.ID.ToString() + ".raw", new byte[] { 0 });

                    byte[] recordedVoice = null;
                    byte[] partnerRecordedVoice = null;

                    lock (RecordedVoiceStreamSyncObject)
                    {
                        recordedVoice = RecordedVoiceStream.ToArray();
                        partnerRecordedVoice = RecordedVoiceStream_Partner.ToArray();
                    }

                    DateTime t1 = DateTime.Now;

                    #region Sync both from start
                    if (RecordedVoiceStartTime_Partner > RecordedVoiceStartTime)
                    {
                        int offset = (int)(RecordedVoiceStartTime_Partner.Value.Subtract(RecordedVoiceStartTime.Value).TotalSeconds * 8000.0);
                        Logger.WriteView("SaveCall Skip {0} bytes on save voice, dialogID:{1}", offset, DialogID);
                        recordedVoice = recordedVoice.Skip(offset).ToArray();
                    }
                    else
                    {
                        int offset = (int)(RecordedVoiceStartTime.Value.Subtract(RecordedVoiceStartTime_Partner.Value).TotalSeconds * 8000.0);
                        Logger.WriteView("SaveCall Skip {0} bytes on save voice, dialogID:{1}", offset, DialogID);
                        partnerRecordedVoice = partnerRecordedVoice.Skip(offset).ToArray();
                    }
                    #endregion

                    if (RecordedVoiceStartTime.HasValue && RecordedVoiceStartTime_Partner.HasValue && (Config.Default.TalkingVoiceRecordMode ==
                        TalkingVoiceRecordMode.Merged || Config.Default.TalkingVoiceRecordMode == TalkingVoiceRecordMode.All))
                    {
                        var voice = Folder.Audio.AudioUtility.Mix(recordedVoice, partnerRecordedVoice);
                        DateTime t2 = DateTime.Now;

                        Logger.WriteDebug("SaveCall '{0}': mixed source: {3}, mixed time: {1} (ms), size: {2} (Bytes), startTime1 '{4}', startTime2 '{5}'",
                               DialogID, t2.Subtract(t1).TotalMilliseconds, partnerRecordedVoice.Length, 1, RecordedVoiceStartTime.Value.ToString("mm:ss.fff"), RecordedVoiceStartTime_Partner.Value.ToString("mm:ss.fff"));

                        VoiceFileManager.SaveVoice(Call.ID.ToString() + ".raw", voice);
                    }

                    if (Config.Default.TalkingVoiceRecordMode == TalkingVoiceRecordMode.Separately || Config.Default.TalkingVoiceRecordMode == TalkingVoiceRecordMode.All)
                    {
                        VoiceFileManager.SaveVoice(Call.ID.ToString() + ".#1.raw", recordedVoice);
                        VoiceFileManager.SaveVoice(Call.ID.ToString() + ".#2.raw", partnerRecordedVoice);
                    }

                    Logger.WriteDebug("SaveTalkingVoice - Voice record is saved for dialog:{0}, Guid:{1}, DisconnectTime:{2}, DisconnectCause:{3}", DialogID, Call.ID, Call.DisconnectTime, Call.DisconnectCause);
                }
                catch (IOException ex)
                {
                    if (ex.Message.Contains("There is not enough space on the disk."))
                        Logger.WriteCritical("SaveCall '{0}': Error writing voice on hard, There is not enough space on the disk.", DialogID);
                    else
                        Logger.Write(ex);
                }

                RecordedVoiceStream = null;
                RecordedVoiceStream_Partner = null;
                RecordedVoiceStartTime = null;
                RecordedVoiceStartTime_Partner = null;
            }
        }

        #endregion

        #region Events
        internal event EventHandler<StatusTimeoutEventArgs> StatusTimeout;
        public event EventHandler<StatusChangedEventArgs> StatusChanged;
        #endregion

        private DialogStatus _Status;
        private Dictionary<string, object> dialogStates = new Dictionary<string, object>();

        internal string ForwardReferViaBranch;
        internal SipAccount FromAccount;

        //SIP server calculates this parameters in IVR mode to generate RTP packet
        internal int Sequence;// Voice chunk sequence.
        internal int TimeStamp;
        internal int SSRC;

        /* SIP server needs first timeStamp (FTS) to find the position of RTP packet in the memory stream of voice record -> TS = MemoryStream.Length + FTS + Payload.length
         * 
         * Unlike RTP standard, some clients change their TS,SSRC and SN after Holding/UnHolding. So changing SSRC is not supported in our 
         * recording algorithm and we save client SSRC to not record streams with other SSRC.
         */
        internal int SN;
        //internal int PreviousSN;
        internal long TS;
        internal long RemoteSSRC;
        internal long CurrentRemoteSSRC;
        internal long FTS; //First time stamp to find packet position in memoryStream
        internal long Skipped_samples;
        internal long Hold_samples;

        internal object DtmfDetectingSyncObject = new object();
        internal bool DtmfDetecting;// True: incoming DTMF  /   False: no DTMF detected 
        internal DateTime DtmfDetectingStartTime;
        internal SipMessage InviteMessage;
        internal SipMessage ReferMessage;
        internal string ByeMessage;
        internal IPEndPoint ByeTarget;
        internal int SendByeRetry = 1;
        internal int SendInviteRetry = 1;
        internal int SendOKRetry = 1;
        internal string ForwardInviteViaBranch;
        internal SipFieldTo RingingTo; // Used for reject call

        internal byte TryForPoll = 0;

        [XmlIgnore]
        public DisconnectCause DisconnectCause
        {
            get
            {
                return (DisconnectCause)Call.DisconnectCause;
            }
            set
            {
                Call.DisconnectCause = (int)value;
            }
        }
        public string Extension
        {
            get
            {
                return Call.Extension;
            }
            set
            {
                Call.Extension = value;
            }
        }
        public string DialogID
        {
            get
            {
                return Call.DialogID;
            }
            set
            {
                Call.DialogID = value;
            }
        }
        public string CalleeID
        {
            get
            {
                return Call.CalleeID;
            }
            set
            {
                Call.CalleeID = value;
            }
        }
        public string CallerID
        {
            get
            {
                return Call.CallerID;
            }
            set
            {
                Call.CallerID = value;
            }
        }
        public DateTime CallTime
        {
            get
            {
                return Call.CallTime;
            }
            set
            {
                Call.CallTime = value;
            }
        }
        public DialogType DialogType
        {
            get
            {
                return (DialogType)Call.Type;
            }
            set
            {
                Call.Type = (int)value;
            }
        }
        public DialogStatus Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;

                if (StatusChanged != null)
                    StatusChanged(this, new StatusChangedEventArgs());

                //if (Config.Default.LogChangeStatusMessage)
                //    Logger.WriteView("[{0}]={1}", Status, DialogID);

                StatusChangeTime = DateTime.Now;
            }
        }
        public readonly Call Call;
        public SipAccount ToAccount;
        public object this[string key]
        {
            get { return dialogStates[key]; }
            set { dialogStates[key] = value; }
        }
        public DateTime StatusChangeTime;
        public string Keys
        {
            get;
            set;
        }
        public SdpFieldMedia.MediaType MediaType = SdpFieldMedia.MediaType.Audio;

        #region Voice, Recording And Audition
        public RtpNet RtpNet; // a Net instance that created on incoming call (SipClient->HandleInvite) and appropriated a port RTP deal
        public bool AuditionEnabled = false;
        public byte[] AuditionReceivedVoice;
        public byte[] AuditionSentVoice;
        public SipDialog AuditionTarget;
        public byte[] VoiceStream;// Carrying voice chunks and put them for playing
        public int VoiceStreamOffset;// Expose next voice chunk start-point

        public int HoldVoiceOffset = 0;

        [XmlIgnore]
        public bool RecordVoice = true;
        public object RecordedVoiceStreamSyncObject = new object();
        public MemoryStream RecordedVoiceStream;// Appendable memory for collecting incoming voice chunks
        public DateTime? RecordedVoiceStartTime;

        //Operator dispart from the CALL when operator clicks on Play Tel (خواندن شماره). So we maintain his RecordedVoice in dialog of subscriber
        public MemoryStream RecordedVoiceStream_Partner;
        public DateTime? RecordedVoiceStartTime_Partner;

        public MemoryStream CollectedFaxStream;// Appendable memory for collecting incoming image(fax) chunks
        public DateTime? CollectedFaxStartTime;
        public byte[] RecordedVoice
        {
            get
            {
                lock (RecordedVoiceStreamSyncObject)
                {
                    if (RecordedVoiceStream == null)
                        return new byte[0];

                    return RecordedVoiceStream.ToArray();
                }

                //using (BinaryReader reader = new BinaryReader(RecordedVoiceStream))
                //{
                //    byte[] result = new byte[RecordedVoiceStream.Length];
                //    reader.Read(result, 0, result.Length);
                //    return result;
                //}
            }
        }

        public MemoryStream PassThroughFaxStream;// Appendable memory for collecting incoming chunks
        #endregion

        #region Divert Properties
        public Schema.Graph BeforeDivertGraph;
        internal bool IsForwardMode
        {
            get
            {
                return FromAccount != null && ToAccount != null;
            }
        }
        internal SipAccount Partner
        {
            get
            {
                return FromAccount != null ? FromAccount : ToAccount;
            }
        }
        public Node BeforeDivertNode;
        public int? WaitForDivertingTimeout;
        public int TransferFailureTime = 0;
        public SafeCollection<DivertTarget> DivertTargets = new SafeCollection<DivertTarget>();
        public DivertTarget CurrentlyDivertTarget;
        public string DivertCallID
        {
            get;
            set;
        }
        public string AgentID
        {
            get;
            set;
        }
        public SipDialog DivertPartner
        {
            get;
            set;
        }
        public string DivertTargetTeam
        {
            get;
            set;
        }
        public DateTime? QueueEnterTime
        {
            get { return Call.QueueEnterTime; }
            set { Call.QueueEnterTime = value; }
        }
        #endregion

        public bool IsInforming = false;
    }

    public class StatusChangedEventArgs : EventArgs
    {
    }

    public class StatusTimeoutEventArgs : EventArgs
    {
    }
}
