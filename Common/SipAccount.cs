using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Net;
using System.ComponentModel;
using Enterprise;

namespace UMSV.Schema
{
    public class SipAccount
    {
        public SipAccount()
        {
            LastCallTime = DateTime.MinValue;
            MaxConcurrentCalls = 1;
            Status = SipAccountStatus.Offline;
            DialogAverageTime = 180;
            DialogAverageTimeSampleCount = 1;
            Previous_DialogID = "notSet";
        }

        public DateTime LastCallTime
        {
            get;
            set;
        }

        public string CalleeID;
        public DateTime? ClientAddressChangeTime;

        public IPEndPoint SipEndPoint
        {
            get;
            set;
        }

        /* This method must be only called when a disconnetCause is arrised from the current call. 
         * Sometimes retransmitted messages from the previous call changes the current call status of the operator !!! 
         * 
         * We handle this situation in HandleCallDisconnect() method and The dialog.disconnected() is called one time for each dialog.
         * The checking for previous DialogID in this method is only for assurance, if someone calls dialog.disconnected() directly. 
         */
        public void DialogDisconnected(string Dialogid)
        {
            LastCallEndedTime = DateTime.Now;
            if (MaxConcurrentCalls == 1 && Status != SipAccountStatus.Offline && Status != SipAccountStatus.DND)
            {
                if (Previous_DialogID == Dialogid)
                {
                    Logger.WriteDebug("SipAccount -> A message from the previous call. The current status of the operator:{0} is not changed. Status:{1}, DialogID:{2}", UserID, Status.ToString(), Dialogid);
                }
                else
                {
                    Logger.WriteDebug("SipAccount -> Operator {0} status is changed to idle", UserID);
                    Status = SipAccountStatus.Idle;
                }

                // To protect current call SipAccountStatus from the remaining messages of previous call.
                Previous_DialogID = Dialogid;
            }
        }

        public int TemporarilyUnavailableCount = 0;

        public string UserID
        {
            get;
            set;
        }

        public Folder.User FolderUser;

        public string DisplayName
        {
            get;
            set;
        }

        public string Comment
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }

        public DateTime? RegisterTime
        {
            get;
            set;
        }

        public DateTime? ExpireTime
        {
            get;
            set;
        }

        public DateTime? LastCallEndedTime
        {
            get;
            set;
        }

        public string Contact
        {
            get;
            set;
        }

        public string MatchRule
        {
            get;
            set;
        }

        public int MaxConcurrentCalls
        {
            get;
            set;
        }

        public SipAccountStatus Status
        {
            get;
            set;
        }

        // USE: Do not change current call status if a meesage from previous Call is comming
        public string Previous_DialogID
        {
            get;
            set;
        }

        public int DialogAverageTime
        {
            get;
            set;
        }

        public int DialogAverageTimeSampleCount
        {
            get;
            set;
        }

        public string EavesdropperUserId
        {
            get;
            set;
        }
    }
}
