using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using UMSV.Schema;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        public void DisconnectCall(SipDialog dialog)
        {
            SipServer.DisconnectCall(dialog);
        }

        public void RecordVoice(SipDialog dialog, string boxNo)
        {
            try
            {
                UmsDataContext dc = new UmsDataContext();
                MailboxMessage mailbox = new MailboxMessage()
                {
                    BoxNo = boxNo,
                    Data = dialog.RecordedVoice,
                    ReceiveTime = DateTime.Now,
                    Sender = dialog.CallerID,
                    Type = (byte)MailboxMessageType.New,
                };
                dc.MailboxMessages.InsertOnSubmit(mailbox);
                dc.SubmitChanges();
            }
            catch (SqlException ex)
            {
                Logger.WriteError("Error in Saving recorded voice to mailbox {0}, reslut:{1}", boxNo, ex.Message);
            }
        }

        public object GetCorePropertyVoice(SipDialog dialog, string voiceName)
        {
            switch (voiceName)
            {
                case "QueueIndex":
                    {
                        var queueIndex = DivertQueue.Take(DivertQueue.IndexOf(dialog)).Count(d => d.DivertTargets.Any(t => dialog.DivertTargets.Contains(t)));
                        return new NumberVoice(queueIndex + 1, NumberSuffix.ome);
                    }

                case "EstimatedWaitTime":
                    {
                        int index = DivertQueue.Take(DivertQueue.IndexOf(dialog)).Count(d => d.DivertTargets.Any(t => dialog.DivertTargets.Contains(t)));
                        int average = (int)dialog.DivertTargets.Average(d => d.DialogAverageTime);
                        return (index + 1) * average / 60 / dialog.DivertTargets.Count;
                    }

                default:
                    return null;
            }
        }
    }
}
