using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Folder;
using Enterprise;
using System.IO;
using UMSV.Schema;
using System.Reflection;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading;
using System.Data.SqlClient;
using Folder.EMQ;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        void TrackInvokeNode(SipDialog dialog)
        {
            if (dialog.CurrentNode == null)
                return;
            var node = dialog.CurrentNode.AsInvokeNode;

            try
            {
                node.InvokeTimes += 1;
                if (node.InvokeTimes > Config.Default.LoopedInvokeMaxTimes && node.LastInvokeTime.HasValue &&
                    DateTime.Now.Subtract(node.LastInvokeTime.Value).TotalMilliseconds < Config.Default.LoopedInvokeMinInterval)
                {
                    Logger.WriteError("Unlimitted Loop ocuuring on dialog :{0} ...", dialog.DialogID);
                    DisconnectCall(dialog);
                    return;
                }
                node.LastInvokeTime = DateTime.Now;

                object tragetInstance = null;

                if (string.IsNullOrWhiteSpace(node.Function))
                {
                    Logger.WriteWarning("Method is empty in Addins {0}, node ID:{1}", dialog.Graph.Description, node.ID);
                    return;
                }

                MethodInfo method = null;
                if (dialog.GraphAddins != null)
                {
                    method = dialog.GraphAddins.GetType().GetMethod(node.Function);
                    if (method != null)
                        tragetInstance = dialog.GraphAddins;
                }

                if (method == null && node.Function != "Start" && node.Function != "Stop")
                {
                    method = this.GetType().GetMethod(node.Function);
                    if (method != null)
                        tragetInstance = this;
                    else
                    {
                        Logger.WriteError("Method {0} not found in Addins {1}.", node.Function, dialog.Graph.Description);
                        return;
                    }
                }

                ArrayList argValues = new ArrayList();
                var args = node.Arg.Select(a => a.Value).ToArray();
                ParameterInfo[] @params = method.GetParameters();

                foreach (ParameterInfo param in @params)
                {
                    if (param.ParameterType == typeof(SipDialog))
                    {
                        argValues.Add(dialog);
                        continue;
                    }

                    InvokeNodeArg arg = node.GetArgByName(param.Name);
                    if (arg == null)
                    {
                        Logger.Write(LogType.Error, "Argument '{0}' is expected!", param.Name);
                        argValues.Add(null);
                    }
                    else
                    {
                        object value = Convert.ChangeType(arg.Value, param.ParameterType);
                        argValues.Add(value);
                    }
                }

                object result = null;
                try
                {
                    result = method.Invoke(tragetInstance, argValues.ToArray());
                }
                catch (Exception ex)
                {
                    Logger.Write(ex, "Error Invoking Method {0} in Addins {1}.", node.Function, dialog.Graph.Description);
                }
                dialog.CurrentNodeID = node.AsInvokeNode.GetResultTarget(result);
                TrackCurrentNode(dialog);
            }
            catch (Exception ex)
            {
                Logger.Write(LogType.Exception, ex, "Error while Invokeing method '{0}', graph '{1}'", node != null ? node.Function : "NODE IS NULL", dialog.Graph.Description);
            }

        }

        public object GetCorePropertyVoice(SipDialog dialog, string voiceName)
        {
            switch (voiceName)
            {
                case "QueueIndex":
                    //return new NumberVoice(QueueIndex(dialog), NumberSuffix.ome);
                    return new NumberVoice(QueueIndex(dialog)); // for Rasht
                case "QueueIndexSimple":
                    return new NumberVoice(Math.Max(1, QueueIndex(dialog) - 1));

                case "EstimatedWaitTime":
                    return CalculateEstimatedWaitTime(dialog);

                case "InfoTableRecordData":
                    return PlayInfoTableRecordData(dialog);

                case "CurrentCodeStatusVoiceMessage":
                    return PlayCurrentCodeStatusVoiceMessage(dialog);

                case "PlayFromPublicMailbox":
                    return PlayFromPublicMailbox(dialog);

                case "Extension":
                    return dialog.Extension;

                case "FollowUpCode":
                    return (int)dialog[Constants.DialogState_FollowupCode];

                case "PlayCode":
                    return dialog.Keys.Replace("*", "").Replace("#", "");

                case "PlayAnswerMessage":
                    return dialog[Constants.DialogState_AnswerVoice];

                default:
                    return null;
            }
        }

        private object CalculateEstimatedWaitTime(SipDialog dialog)
        {
            var queueIndex = QueueIndex(dialog);
            int average = (int)dialog.DivertTargets.Where(d => d.Account.DialogAverageTime > 15).Average(d => d.Account.DialogAverageTime);
            average = Math.Max(average, 15);
            var time = queueIndex * average / 60 / dialog.DivertTargets.Where(t => t.Account.Status != SipAccountStatus.Offline).Count();
            time = Math.Max(1, time);
            Logger.WriteDebug("QueueIndex, queueentertime:{3} dialog from {0}, call id:{1} time:{2}", dialog.CallerID, dialog.DialogID, time, dialog.QueueEnterTime.Value.TimeOfDay);
            return time;
        }

        public void SaveUserData(SipDialog dialog)
        {
            try
            {
                Logger.WriteInfo("Saving new UserData .");

                using (UMSV.UmsDataContext dc = new UmsDataContext())
                {
                    UserData newUserData = new UserData()
                    {
                        ID = Guid.NewGuid(),
                        CalleeID = dialog.CalleeID,
                        CallerID = dialog.CallerID,
                        CallTime = dialog.CallTime,
                        DialogID = dialog.DialogID,
                        Data = dialog.Call.Extension,
                        GraphID = dialog.Call.GraphID.Value
                    };

                    dc.UserDatas.InsertOnSubmit(newUserData);
                    dc.SubmitChanges();
                }

                Logger.WriteInfo("Saved UserData successfuly .");
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        public void SaveLineDigits(SipDialog dialog)
        {
            var keys = dialog.Keys.Replace("*", "").Replace("#", "");
            Logger.WriteInfo("SaveLineDigits: {0}", keys);
            dialog.Call.Extension = keys;
        }

        #region Mailbox

        public enum CheckFollowupCodeResult
        {
            Answered,
            InvalidCode,
            NoAnswerYet,
            MaxTry,
        }

        public CheckFollowupCodeResult CheckFollowupCode(SipDialog dialog, string boxNo)
        {
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    var message = dc.MailboxMessages.FirstOrDefault(m => m.BoxNo == boxNo && m.Type == (byte)MailboxMessageType.Answer && m.FollowupCode.HasValue && m.FollowupCode.ToString() == dialog.Keys.Replace("#", "").Replace("*", ""));
                    if (message != null)
                    {
                        dialog[Constants.DialogState_AnswerVoice] = message.Data;
                        message.ExpireDate = DateTime.Now;

                        Logger.WriteDebug("CheckFollowupCode Answered box:{0}, followupCode:{1}, dialog:{2}, callerID:{3}", boxNo, message.FollowupCode, dialog.DialogID, dialog.CallerID);
                        return CheckFollowupCodeResult.Answered;
                    }

                    if (dc.MailboxMessages.Any(m => m.BoxNo == boxNo && m.Type == (byte)MailboxMessageType.Ask && m.FollowupCode.HasValue && m.FollowupCode.ToString() == dialog.Keys.Replace("#", "").Replace("*", "")))
                    {
                        Logger.WriteDebug("CheckFollowupCode NoAnswerYet box:{0}, followupCode:{1}, dialog:{2}, callerID:{3}", boxNo, dialog.Keys, dialog.DialogID, dialog.CallerID);
                        return CheckFollowupCodeResult.NoAnswerYet;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            return CheckFollowupCodeResult.InvalidCode;
        }

        public bool AskCheckMailbox(SipDialog dialog, string boxNo)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                var mailbox = dc.Mailboxes.FirstOrDefault(m => m.BoxNo == boxNo);
                return mailbox != null;
            }
        }

        public void RecordVoice(SipDialog dialog, string boxNo)
        {
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    MailboxMessage message = new MailboxMessage()
                    {
                        BoxNo = boxNo,
                        ReceiveTime = DateTime.Now,
                        Sender = dialog.CallerID,
                        Type = (byte)MailboxMessageType.New,
                    };

                    if (dialog.RecordedVoiceStream != null)
                        message.Data = dialog.RecordedVoiceStream.ToArray();

                    dc.MailboxMessages.InsertOnSubmit(message);
                    dc.SubmitChanges();
                }
            }
            catch (SqlException ex)
            {
                Logger.WriteError("Error in Saving recorded voice to mailbox {0}, reslut:{1}", boxNo, ex.Message);
            }
        }

        public void RecordQuestion(SipDialog dialog, string boxNo)
        {
            try
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    int? followupCode = dc.MailboxMessages.Max(m => m.FollowupCode);
                    var mailbox = dc.Mailboxes.FirstOrDefault(m => m.BoxNo == boxNo);

                    if (followupCode.HasValue)
                        followupCode++;
                    else
                        followupCode = mailbox.FollowupCodeStart;

                    MailboxMessage message = new MailboxMessage()
                    {
                        BoxNo = boxNo,
                        ReceiveTime = DateTime.Now,
                        Sender = dialog.CallerID,
                        Type = (byte)MailboxMessageType.Ask,
                        FollowupCode = followupCode,
                    };

                    if (dialog.RecordedVoiceStream != null)
                        message.Data = dialog.RecordedVoiceStream.ToArray();

                    dc.MailboxMessages.InsertOnSubmit(message);
                    dc.SubmitChanges();

                    dialog[Constants.DialogState_FollowupCode] = message.FollowupCode;
                    Logger.WriteDebug("RecordQuestion box:{0}, followupCode:{1}, dialog:{2}, callerID:{3}", boxNo, message.FollowupCode, dialog.DialogID, dialog.CallerID);
                }
            }
            catch (SqlException ex)
            {
                Logger.WriteError("Error in Saving recorded voice to mailbox {0}, reslut:{1}", boxNo, ex.Message);
            }
        }

        object PlayFromPublicMailbox(SipDialog dialog)
        {
            var boxNo = dialog.CurrentNode.AsPlayNode.BoxNo;

            using (UmsDataContext dc = new UmsDataContext())
            {
                var mailbox = dc.Mailboxes.FirstOrDefault(m => m.BoxNo == boxNo);
                if (mailbox != null && ((MailboxType)mailbox.Type) == MailboxType.Public)
                {
                    var messages = dc.MailboxMessages.Where(m => m.BoxNo == boxNo && m.Type == (byte)MailboxMessageType.PublicMessage);
                    var count = messages.Count();
                    if (count == 0)
                    {
                        Logger.WriteWarning("PlayFromPublicMailbox, boxNo {0} has no public message.", boxNo);
                        return null;
                    }

                    var index = new Random().Next(0, count);
                    Logger.WriteDebug("PlayFromPublicMailbox, boxNo:{0}, index: {1}/{2}", boxNo, index + 1, count);
                    return messages.Skip(index).First().Data.ToArray();
                }
                else
                {
                    Logger.WriteWarning("PlayFromPublicMailbox, boxNo {0} not found or is not public mailbox.", boxNo);
                    return null;
                }
            }
        }

        #endregion

        #region Graph Invoke Core Functions

        public void DisconnectCall(SipDialog dialog)
        {
            // dialog.DisconnectCause = DisconnectCause.NormalUnspecified;
            SipServer.DisconnectCall(dialog);
        }

        public void StartFax(SipDialog dialog)
        {
            Logger.WriteInfo("StartFax ID:{0}", dialog.DialogID);
            SipServer.StartFax(dialog);
        }


        public enum SavePollResult
        {
            Invalid,
            Failed,
            Success
        }

        public SavePollResult SavePoll(SipDialog dialog, string questionId, string agentMode)
        {
            //--------------------Mobinnet------------------------------------------

            try
            {
                Logger.WriteInfo("Saving new poll...");
                Logger.WriteInfo("TryForPoll is {0}", dialog.TryForPoll);

                agentMode = agentMode == null ? string.Empty : agentMode;
                Logger.WriteInfo("agentMode is {0}", agentMode);

                var keys = dialog.Keys.Replace("*", "").Replace("#", "");
                Logger.WriteInfo("dialog is {0}", keys);

                int answerIndex = int.Parse(keys);
                Logger.WriteInfo("answerIndex is {0}", answerIndex);


                int qId = int.Parse(questionId);
                Logger.WriteInfo("qId is {0}", qId);

                //Logger.WriteInfo("QuestionId is {0} , AnswerIndex is {1}", qId, answerIndex);

                if (dialog.TryForPoll == 0 || dialog.TryForPoll == 1)
                {
                    if (!(answerIndex >= 1 && answerIndex <= 5))
                    {
                        Logger.WriteError("Out of range .");
                        dialog.TryForPoll++;

                        if (dialog.TryForPoll > 1)
                            return SavePollResult.Failed; // khodahafez

                        return SavePollResult.Invalid; // error please try again
                    }
                    else
                    {
                        using (UmsDataContext dc = new UmsDataContext())
                        {
                            List<PollAnswer> answers = dc.PollAnswers.Where(t => t.PollQuestionId == qId).OrderBy(t => t.Id).ToList();

                            if (answers != null && dc.PollQuestions.Any(t => t.Id == qId) && answers.Count >= (answerIndex - 1))
                            {
                                PollInfo pollInfo = new PollInfo();

                                pollInfo.CallerId = dialog.CallerID;
                                pollInfo.CalleeId = agentMode.ToLower() == "true" ? dialog.AgentID : dialog.CalleeID;
                                pollInfo.DateAnswered = DateTime.Now;
                                pollInfo.PollQuestionId = qId;
                                pollInfo.PollAnswerId = answers[answerIndex - 1].Id;
                                pollInfo.DialogId = dialog.DialogID;

                                dc.PollInfos.InsertOnSubmit(pollInfo);
                                dc.SubmitChanges();

                                Logger.WriteInfo("PollInfo Saved({0}): Question = {1}, Answer = {2}", dialog.CallerID, qId, answers[answerIndex - 1].Id);

                                return SavePollResult.Success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);

            }

            return SavePollResult.Failed; // khodahafez

            //---------------------------------Other----------------------------------

            //try
            //{
            //    agentMode = agentMode == null ? string.Empty : agentMode;

            //    int qId;
            //    if (!int.TryParse(questionId, out qId))
            //    {
            //        Logger.WriteError("Question ID: {0} is not valid.", questionId);
            //        return SavePollResult.Failed;
            //    }

            //    using (UmsDataContext dc = new UmsDataContext())
            //    {
            //        int answerIndex = int.Parse(dialog.Keys.Trim('*').Trim('#'));
            //        List<PollAnswer> answers = dc.PollAnswers.Where(t => t.PollQuestionId == qId)
            //                                          .OrderBy(t => t.Id)
            //                                          .ToList();

            //        if (answers != null && dc.PollQuestions.Any(t => t.Id == qId) && answers.Count >= (answerIndex - 1))
            //        {
            //            PollInfo pollInfo = new PollInfo()
            //            {
            //                CallerId = dialog.CallerID,
            //                CalleeId = agentMode.ToLower() == "true" ? dialog.AgentID : dialog.CalleeID,
            //                DateAnswered = DateTime.Now,
            //                PollQuestionId = qId,
            //                PollAnswerId = answers[answerIndex - 1].Id
            //            };
            //            dc.PollInfos.InsertOnSubmit(pollInfo);
            //            dc.SubmitChanges();
            //            Logger.WriteInfo("PollInfo Saved({0}): Question = {1}, Answer = {2}", dialog.CallerID, qId, answers[answerIndex - 1].Id);
            //            return SavePollResult.Success;
            //        }
            //    }
            //    return SavePollResult.Invalid;
            //}
            //catch (Exception ex)
            //{
            //    Logger.Write(ex);
            //    return SavePollResult.Failed;
            //}
        }

        public void StartAudition(SipDialog dialog)
        {
            string operatorID = dialog.Keys.Replace("*", "").Replace("#", "");
            var auditDialog = SipServer.Dialogs.FirstOrDefault(d => (d.ToAccount != null && d.ToAccount.UserID == operatorID) ||
                (d.FromAccount != null && d.FromAccount.UserID == operatorID));
            if (auditDialog != null)
            {
                Logger.WriteDebug("StartAudition, operaotrID: {0}, dialog ID:{1}", operatorID, auditDialog.DialogID);
                auditDialog.AuditionEnabled = true;
                auditDialog.AuditionTarget = dialog;

                dialog.Sequence = (short)new Random().Next(0, 255);
                dialog.TimeStamp = (int)DateTime.Now.Ticks;
                dialog.SSRC = new Random().Next(0, int.MaxValue);

            }
            else
                Logger.WriteDebug("StartAudition, operaotrID: {0} not found.", operatorID);
        }

        private int QueueIndex(SipDialog dialog)
        {
            var accounts = dialog.DivertTargets.Select(dt => dt.Account.UserID);
            var previousQueuedDialogs = DivertQueue.Take(DivertQueue.IndexOf(dialog));
            int index = 1;
            foreach (var preDialog in previousQueuedDialogs)
            {
                if (preDialog.DivertTargets.Any(pd => accounts.Contains(pd.Account.UserID)))
                    index++;
            }

            Logger.WriteDebug("QueueIndex, dialog from {0}, call id:{1} index:{2} previousQueuedDialogs:{3}", dialog.CallerID, dialog.DialogID, index, previousQueuedDialogs.Count());
            return index;
            //int index = accounts.Select(a => previousQueuedDialogs.Where(d => d.DivertTargets.Select(dt => dt.Account).Contains(a)).Count()).Min();
            //return index + 1;
        }

        public void SaveKeysInExtension(SipDialog dialog)
        {
            dialog.Extension = dialog.Keys;
        }

        public int CheckCallerSpecialPhone(SipDialog dialog)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                var item = dc.SpecialPhones.FirstOrDefault(p => p.Number == dialog.CallerID);
                if (item == null)
                    return 0;
                else
                    return item.Type;
            }
        }

        #region Info Table

        private object PlayInfoTableRecordData(SipDialog dialog)
        {
            var foundedRecord = (InfoTableRecord)dialog[Constants.DialogState_InfoTableFoundedRecord];

            try
            {
                var columns = foundedRecord.Data.Split(',', '\t', ';');
                int foundedRecordColumnIndex = (int)dialog[Constants.DialogState_InfoTableFoundedRecordColumnIndex];

                if (columns.Length > foundedRecordColumnIndex)
                {
                    int res;
                    var value = columns[foundedRecordColumnIndex];
                    foundedRecordColumnIndex++;
                    dialog[Constants.DialogState_InfoTableFoundedRecordColumnIndex] = foundedRecordColumnIndex;

                    if (string.IsNullOrEmpty(value))
                        return null;

                    if (Regex.IsMatch(value, @"^\d{1,2}:\d{1,2}$"))
                        return TimeSpan.Parse(value.Split(' ')[0]);

                    else if (Regex.IsMatch(value, @"\d+/\d+/\d+"))
                        return PersianDateTime.PersianToGregorian(value).Value;

                    else if (value.IndexOf("-") > -1 || value.StartsWith("0") || value.Length > Constants.VoiceCodeMaxLengthAsNumber)
                        return value.Replace("-", "");

                    else if (int.TryParse(value, out res))
                        return int.Parse(value);
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "PlayInfoTableRecordData, data:{0}, dialog: {1}", foundedRecord.Data, dialog.DialogID);
            }
            return null;
        }

        public InfoTableResult CheckAuthentication(SipDialog dialog, string AuthenticationType, string InfoTable, bool Repeat)
        {
            Logger.WriteDebug("CheckAuthentication Dialog:{3}, AuthenticationType:{0}, InfoTable:{1}, Repeat:{2}", AuthenticationType, InfoTable, Repeat, dialog.DialogID);

            Guid infoTableID;
            if (!Guid.TryParse(InfoTable, out infoTableID))
                return InfoTableResult.RecordNotFound;

            dialog[Constants.DialogState_InfoTableID] = infoTableID;

            if (AuthenticationType == "ByCode")
                return InfoTableResult.AskForCode;

            using (UmsDataContext dc = new UmsDataContext())
            {
                dialog[Constants.DialogState_InfoTableFoundedRecord] = dc.InfoTableRecords.FirstOrDefault(r => r.InfoTable == infoTableID && r.ID == dialog.CallerID);
                dialog[Constants.DialogState_InfoTableFoundedRecordColumnIndex] = 0;
                if (dialog[Constants.DialogState_InfoTableFoundedRecord] == null)
                    return InfoTableResult.RecordNotFound;
                else
                {
                    return InfoTableResult.RecordFound;
                }
            }
        }

        public InfoTableResult CheckInfoTableCode(SipDialog dialog)
        {
            Logger.WriteDebug("CheckInfoTableCode Dialog:{0}, Code:{1}, InfoTable: {2}", dialog.DialogID, dialog.Keys, dialog[Constants.DialogState_InfoTableID]);
            using (UmsDataContext dc = new UmsDataContext())
            {
                dialog[Constants.DialogState_InfoTableFoundedRecord] = dc.InfoTableRecords.FirstOrDefault(r => r.InfoTable == (Guid)dialog[Constants.DialogState_InfoTableID] && r.ID == dialog.Keys);
                dialog[Constants.DialogState_InfoTableFoundedRecordColumnIndex] = 0;
                if (dialog[Constants.DialogState_InfoTableFoundedRecord] == null)
                    return InfoTableResult.RecordNotFound;
                else
                    return InfoTableResult.RecordFound;
            }
        }

        public enum InfoTableResult
        {
            RecordFound,
            RecordNotFound,
            AskForCode,
        }
        #endregion

        #region Code Status

        private object PlayCurrentCodeStatusVoiceMessage(SipDialog dialog)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                Logger.WriteInfo("Code Status Node -> Status: {0}, CodeStatus: {1}", (dialog[Constants.DialogState_CodeStatusRecord] as CodeStatusRecord).Status, (dialog[Constants.DialogState_CodeStatusRecord] as CodeStatusRecord).CodeStatus);

                var message = dc.CodeStatusVoiceMessages.FirstOrDefault(m => m.Status == (dialog[Constants.DialogState_CodeStatusRecord] as CodeStatusRecord).Status
                                                                          && m.CodeStatus.Value == (dialog[Constants.DialogState_CodeStatusRecord] as CodeStatusRecord).CodeStatus);
                if (message != null)
                    return message.Voice;
                else
                    return null;
            }
        }

        public CheckCodeStatusResult CheckCodeStatus(SipDialog dialog, short CodeStatusID, string SqlServer, string AuthenticationType, string Username, string Password, string TableName, string SpName, string CodeField, string StatusField, string Catalog)
        {
            using (UmsDataContext dc = new UmsDataContext())
            {
                dialog[Constants.DialogState_CodeStatusRecord] = dc.CodeStatusRecords.FirstOrDefault(c => c.CodeStatus == CodeStatusID && c.Code == dialog.Keys);
                if (dialog[Constants.DialogState_CodeStatusRecord] == null)
                    return CheckCodeStatusResult.InvalidCode;
                else
                    return CheckCodeStatusResult.ValidCode;
            }
        }

        public enum CheckCodeStatusResult
        {
            MaxTry,
            InvalidCode,
            ValidCode,
        }

        #endregion

        #endregion

        #region Custom Functions

        public string GetCurrentDialogs()
        {
            return MyXmlSerializer.Serialize(SipServer.Dialogs);
        }

        public string GetCurrentAccounts()
        {
            return MyXmlSerializer.Serialize(SipServer.Accounts);
        }

        //public void Dial(string calleeID)
        //{
        //    SipServer.Dial(calleeID);
        //}

        #endregion
    }
}
