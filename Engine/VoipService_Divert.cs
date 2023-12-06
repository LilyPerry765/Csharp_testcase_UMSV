using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enterprise;
using UMSV.Schema;
using System.Text.RegularExpressions;
using Folder;
using System.Threading;

namespace UMSV.Engine
{
    public partial class VoipService
    {
        System.Threading.Timer CheckDivertQueueTimer;
        const int CheckDivertQueueInterval = 500;

        void CheckDisconnectedDialogToAccountForDivertQueue(SipDialog dialog)
        {
            try
            {
                if (DivertQueue.Contains(dialog))
                    DivertQueue.Remove(dialog);

                if (dialog.DivertPartner != null) // RTP Proxy
                {
                    if (dialog.ToAccount == null) // Source
                    {
                        Logger.WriteInfo("Disconnecting Target Call on transfer mode, ConnectTime:{0}, dialog type:{1}", dialog.DivertPartner.Call.CallTime, dialog.DialogType);
                        if (dialog.DivertPartner.Call.AnswerTime != DateTime.MinValue)
                            SipServer.DisconnectCallOnTransferMode(dialog.DivertPartner);
                        else
                            SipServer.CancelDialog(dialog.DivertPartner);
                    }
                    else // Target
                    {
                        Logger.WriteInfo("Continue Source Call after end of transfering.");
                        var sourceDialog = dialog.DivertPartner;

                        if (sourceDialog.BeforeDivertGraph != null)
                        {
                            sourceDialog.Graph = sourceDialog.BeforeDivertGraph;
                            sourceDialog.CurrentNode = sourceDialog.BeforeDivertNode;
                        }
                        SipServer.StopPlayVoice(sourceDialog);

                        if (sourceDialog.CurrentNode != null && sourceDialog.CurrentNode.AsDivertNode != null)
                        {
                            switch ((DisconnectCause)dialog.Call.DisconnectCause)
                            {
                                case DisconnectCause.UserBusy:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.BusyNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.UnallocatedNumber:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.UnallocatedNumberNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.NoUserResponding:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                case DisconnectCause.SubscriberAbsent:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.SubscriberAbsentNode ?? sourceDialog.CurrentNode.AsDivertNode.FailureNode;
                                    break;

                                default:
                                    sourceDialog.CurrentNodeID = sourceDialog.CurrentNode.AsDivertNode.TargetNode;
                                    break;
                            }
                            TrackCurrentNode(sourceDialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
        }

        void SipServer_TransferFailed(object sender, TransferFailedEventArgs e)
        {
            try
            {
                #region Check and remove failed target to not transfering call to it again.
                try
                {
                    if (e.Dialog.CurrentlyDivertTarget != null)
                    {
                        e.Dialog.DivertTargets.Remove(e.Dialog.CurrentlyDivertTarget);
                        e.Dialog.CurrentlyDivertTarget = null;
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteError("Removing CurrentlyDivertTarget from dialog on transfer failure failed, dialog:{0}, error:{1}", e.Dialog.DialogID, ex.Message);
                }
                #endregion

                if (e.Dialog.DivertPartner == null)
                {
                    Logger.WriteWarning("SipServer_TransferFailed when e.Dialog.DivertPartner is null, DialogID: {0}", e.Dialog.DialogID);
                    return;
                }

                if (e.Dialog.TransferFailureTime > Constants.TransferFailureMaxTime || e.Dialog.DivertPartner.TransferFailureTime > Constants.TransferFailureMaxTime)
                {
                    Logger.WriteError("TransferFailureMaxTime failure count:{0}, Partner failure count:{2} dialog ID:{1}", e.Dialog.TransferFailureTime, e.Dialog.DialogID, e.Dialog.DivertPartner.TransferFailureTime);
                    e.Dialog.CurrentNodeID = e.Dialog.CurrentNode.AsDivertNode.FailureNode;
                    TrackCurrentNode(e.Dialog);
                    return;
                }

                e.Dialog.TransferFailureTime++;
                e.Dialog.DivertPartner.TransferFailureTime++;

                Logger.WriteWarning("SipServer_TransferFailed, callid:{0}, dc cause:{1}, Subscriber Failure Time:{2}", e.Dialog.DialogID, e.Dialog.DivertPartner.DisconnectCause, e.Dialog.TransferFailureTime);
                //CheckDisconnectedDialogToAccountForDivertQueue(e.Dialog);
                switch (e.Dialog.DivertPartner.DisconnectCause)
                {
                    //FIXME: system must act for UserBusy(486) same as TemporarilyUnavailable(480) and forward subscriber to another operator
                    //case DisconnectCause.UserBusy:
                    case DisconnectCause.NoUserResponding:
                    case DisconnectCause.NoAnswerFromUser:
                    case DisconnectCause.UnallocatedNumber:
                    case DisconnectCause.UnknownProblem:
                        CheckDisconnectedDialogToAccountForDivertQueue(e.Dialog);
                        if (e.Dialog.DivertPartner.ToAccount.MaxConcurrentCalls > 1)
                            DisconnectCall(e.Dialog.DivertPartner);
                        break;

                    case DisconnectCause.SubscriberAbsent:
                    default:
                        TrackDivertNode(e.Dialog, true);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex, "SipServer_TransferFailed: {0}", e.Dialog.DialogID);
            }
        }

        DivertTarget ReadyToDivertTarget(SipDialog dialog)
        {
            return dialog.DivertTargets.Where(t =>
                        t.Account.MaxConcurrentCalls != 1 ||
                        (
                            (t.Account.Status == SipAccountStatus.Idle || (t.Account.Status == SipAccountStatus.DND && Config.Default.SendCallToDnd)) &&
                            t.Account.RegisterTime.HasValue &&
                            DateTime.Now.Subtract(t.Account.LastCallTime).TotalMilliseconds > Config.Default.OperatorDivertInterval &&
                            (!t.Account.LastCallEndedTime.HasValue ||
                                DateTime.Now.Subtract(t.Account.LastCallEndedTime.Value).TotalMilliseconds > Config.Default.OperatorDivertInterval))
                                )
                        .OrderBy(t => t.Account.LastCallEndedTime).FirstOrDefault();
        }

        void CheckDivertQueueForNewUser(SipAccount account)
        {
            //using (FolderDataContext dc = new FolderDataContext())
            //{
            //    var teams = (from ur in dc.UserRoles
            //                 join u in dc.Users on ur.UserID equals u.ID
            //                 where u.Username == account.UserID && ur.Role.ParentID == Constants.TeamsRole
            //                 select ur.RoleID.ToString().ToUpper()).ToList();

            //    var queues = DivertQueue.Where(q => !string.IsNullOrEmpty(q.DivertTargetTeam) &&
            //        teams.Contains(q.DivertTargetTeam.ToUpper()));
            //    foreach (var dialog in queues)
            //    {
            //        dialog.DivertTargets = new SafeCollection<DivertTarget>(
            //            ExtractDivertTargetAccounts(dialog, dialog.BeforeDivertNode.AsDivertNode));
            //    }
            //}

            //CheckDivertQueue(null);
        }

        void CheckDivertQueue(object state)
        {
            try
            {
                foreach (var dialog in DivertQueue)
                {
                    try
                    {
                        if (dialog.Status == DialogStatus.DivertingWaitForTargetResponse || dialog.Status == DialogStatus.Talking)
                            continue;

                        var divertTarget = ReadyToDivertTarget(dialog);
                        if (divertTarget != null)
                        {
                            Logger.Write("DivertQueue", "Account '{0}' selected. LastCallTime: '{1}'. LastCallEndedTime: '{2}' dialog ID:{3}",
                                divertTarget.Account.UserID, divertTarget.Account.LastCallTime, divertTarget.Account.LastCallEndedTime, dialog.DialogID);

                            divertTarget.Account.LastCallTime = DateTime.Now;
                            DivertQueue.Remove(dialog);
                            //dialog.Status = DialogStatus.DivertingWaitForTargetResponse; is in transfercall

                            if (dialog.BeforeDivertGraph != null)
                                dialog.Graph = dialog.BeforeDivertGraph;

                            if (dialog.BeforeDivertNode != null)
                                dialog.CurrentNode = dialog.BeforeDivertNode;

                            if (dialog.CurrentNode.AsDivertNode.ProxyMode)
                                SipServer.PlayVoice(dialog, RingingVoice);


                            //Eavesdropping
                            if (!string.IsNullOrEmpty(divertTarget.Account.EavesdropperUserId))
                            {
                                SipDialog targetDialog;
                                SipServer.TransferCall(dialog, divertTarget, dialog.CurrentNode.AsDivertNode.ProxyMode,
                                    dialog.CurrentNode.AsDivertNode.RecordVoice, out targetDialog);

                                Thread.Sleep(50);

                                Logger.WriteImportant("Transfering call to eavesdropper...");

                                SipAccount eavesdropperAccount = SipServer.Accounts.SingleOrDefault(t => t.UserID == divertTarget.Account.EavesdropperUserId);
                                if (eavesdropperAccount != null)
                                    SipServer.TransferCallToEavesdropper(dialog, eavesdropperAccount, targetDialog);
                            }
                            else
                            {
                                SipServer.TransferCall(dialog, divertTarget, dialog.CurrentNode.AsDivertNode.ProxyMode,
                                    dialog.CurrentNode.AsDivertNode.RecordVoice);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Write(ex, "CheckDivertQueue, dialog: {0}", dialog.DialogID);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);
            }
            //finally
            //{
            //    if (CheckDivertQueueTimer != null)
            //        CheckDivertQueueTimer.Change(CheckDivertQueueInterval, -1);
            //}
        }

        void TrackDivertNode(SipDialog dialog, bool insertInQueueHead = false)
        {
            if (dialog.CurrentNode == null)
                return;
            var divertNode = dialog.CurrentNode.AsDivertNode;

            #region Generating DynamicTargetPhone
            if (string.IsNullOrWhiteSpace(divertNode.DynamicTargetPhone))
            {
                if (!string.IsNullOrWhiteSpace(divertNode.TargetTeam))
                {
                    using (FolderDataContext dc = new FolderDataContext())
                    {
                        Guid targetTeam = Guid.Parse(divertNode.TargetTeam);
                        divertNode.DynamicTargetPhone = string.Join(",", (from ur in dc.UserRoles
                                                                          where ur.RoleID == targetTeam
                                                                          join u in dc.Users on ur.UserID equals u.ID
                                                                          select u.Username).ToArray());
                        dialog.DivertTargetTeam = divertNode.TargetTeam;
                    }
                }
                else
                    divertNode.DynamicTargetPhone = divertNode.TargetPhone;
            }
            else
            {
                Logger.WriteImportant("Dynamic target phone is set for outcall in this divert node. target: {0}", divertNode.DynamicTargetPhone);
            }
            #endregion

            if (string.IsNullOrWhiteSpace(divertNode.DynamicTargetPhone))
            {
                Logger.WriteWarning("No Target dynamic phones choosen for Divert Node, node ID:{0}", divertNode.ID);
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FailureNode);
                TrackCurrentNode(dialog);
                return;
            }

            if (Regex.IsMatch(divertNode.TargetPhone, @"\[Extension\]"))
                divertNode.TargetPhone = Regex.Replace(divertNode.TargetPhone, @"\[Extension\]", dialog.Extension.Replace("#", "").Replace("*", ""));

            if (Regex.IsMatch(divertNode.TargetPhone, @"\[CalleeID\]"))
                divertNode.DynamicTargetPhone = Regex.Replace(divertNode.TargetPhone, @"\[CalleeID\]", dialog.CalleeID);

            if (Regex.IsMatch(divertNode.TargetPhone, @"\[Keys\]"))
                divertNode.DynamicTargetPhone = Regex.Replace(divertNode.TargetPhone, @"\[Keys\]", dialog.Keys);

            #region Process CallerID
            if (divertNode.ClearAllSource)
                dialog.CallerID = string.Empty;

            if (divertNode.CallerDeleteFromStart > 0 && dialog.CallerID.Length > divertNode.CallerDeleteFromStart)
                dialog.CallerID = dialog.CallerID.Remove(0, divertNode.CallerDeleteFromStart);

            if (divertNode.CallerDeleteFromEnd > 0 && dialog.CallerID.Length > divertNode.CallerDeleteFromStart)
                dialog.CallerID = dialog.CallerID.Remove(divertNode.CallerDeleteFromEnd, dialog.CallerID.Length - divertNode.CallerDeleteFromEnd);

            if (!String.IsNullOrEmpty(divertNode.CallerPrefix))
                dialog.CallerID = dialog.CallerID.Insert(0, divertNode.CallerPrefix);

            if (!String.IsNullOrEmpty(divertNode.CallerPostfix))
                dialog.CallerID = dialog.CallerID.Insert(dialog.CallerID.Length, divertNode.CallerPostfix);
            #endregion

            #region Process CalleeID
            divertNode.DynamicTargetPhone =
                divertNode.DynamicTargetPhone.Remove(0, Math.Min(divertNode.CalleeDeleteFromStart, divertNode.DynamicTargetPhone.Length));

            if (divertNode.DynamicTargetPhone.Length > divertNode.CalleeDeleteFromEnd)
                divertNode.DynamicTargetPhone = divertNode.DynamicTargetPhone.Substring(0, divertNode.DynamicTargetPhone.Length - divertNode.CalleeDeleteFromEnd);

            divertNode.DynamicTargetPhone = divertNode.DynamicTargetPhone.Insert(0, divertNode.CalleePrefix ?? string.Empty);
            divertNode.DynamicTargetPhone += divertNode.CalleePostfix ?? string.Empty;
            #endregion

            ExtractDivertTargetAccounts(dialog, divertNode);

            if (dialog.DivertTargets.Count == 0 && !Config.Default.QueueTransferIfNoOnlineAccount)
            {
                Logger.WriteInfo("({0}:{1})->No online account found for diverting, Target phone:{2}", dialog.CalleeID, dialog.CallerID, divertNode.DynamicTargetPhone);
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FailureNode);
                TrackCurrentNode(dialog);
                return;
            }

            dialog.QueueEnterTime = DateTime.Now;

            if (DivertQueue.Count == 0)
            {
                dialog.Status = DialogStatus.WaitForDiverting;
                dialog.WaitForDivertingTimeout = divertNode.Timeout;

                var divertTarget = ReadyToDivertTarget(dialog);
                if (divertTarget != null)
                {
                    Logger.Write("DivertQueue", "Account '{0}' selected immediately. LastCallTime: '{1}'. LastCallEndedTime: '{2}', dialog:{3}",
                        divertTarget.Account.UserID, divertTarget.Account.LastCallTime, divertTarget.Account.LastCallEndedTime, dialog.DialogID);

                    divertTarget.Account.LastCallTime = DateTime.Now;
                    if (divertNode.ProxyMode)
                        SipServer.PlayVoice(dialog, RingingVoice);

                    //SipServer.TransferCall(dialog, divertTarget, divertNode.ProxyMode, divertNode.RecordVoice);

                    //Eavesdropping
                    if (!string.IsNullOrEmpty(divertTarget.Account.EavesdropperUserId))
                    {
                        SipDialog targetDialog;
                        SipServer.TransferCall(dialog, divertTarget, dialog.CurrentNode.AsDivertNode.ProxyMode,
                            dialog.CurrentNode.AsDivertNode.RecordVoice, out targetDialog);

                        Thread.Sleep(50);

                        Logger.WriteImportant("Transfering call to eavesdropper...");

                        SipAccount eavesdropperAccount = SipServer.Accounts.SingleOrDefault(t => t.UserID == divertTarget.Account.EavesdropperUserId);
                        if (eavesdropperAccount != null)
                            SipServer.TransferCallToEavesdropper(dialog, eavesdropperAccount, targetDialog);
                    }
                    else
                    {
                        SipServer.TransferCall(dialog, divertTarget, dialog.CurrentNode.AsDivertNode.ProxyMode,
                            dialog.CurrentNode.AsDivertNode.RecordVoice);
                    }

                    //if (divertNode.MaxTalkTime > 0)
                    //    NodeTimeoutManager.NodeTimeouts.Add(dialog, DateTime.Now.AddSeconds(divertNode.MaxTalkTime));
                    return;
                }
            }

            #region Check Queue Is Busy
            if (!string.IsNullOrEmpty(divertNode.BusyNode) &&
                (dialog.DialogType == DialogType.ClientOutgoing || dialog.DialogType == DialogType.GatewayOutgoing)
                && dialog.DivertTargets.Count == 1 && dialog.DivertTargets.First().Account.MaxConcurrentCalls == 1)
            {
                Logger.WriteWarning("Target is busy...");
                dialog.Extension = "BusyQueue";
                dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.BusyNode);
                if (dialog.CurrentNode != null)
                {
                    Logger.WriteInfo("Target is busy, and busy target node is available: {0}", dialog.CurrentNode.ID);
                    TrackCurrentNode(dialog);
                    return;
                }
            }
            #endregion

            #region Check Queue Is Full
            if (!string.IsNullOrEmpty(divertNode.FullQueueNode))
            {
                int queueCount;
                if (!string.IsNullOrEmpty(divertNode.TargetTeam))
                    queueCount = DivertQueue.Where(q => q.DivertTargetTeam == divertNode.TargetTeam).Count();
                else
                    queueCount = DivertQueue.Count();

                Logger.WriteView("Queue count: {0}", queueCount);
                if ((divertNode.QueueSize != 0 && queueCount >= divertNode.QueueSize) ||
                (divertNode.QueueSizePerOnlineUsers != 0 && queueCount >= dialog.DivertTargets.Count(t => t.Account.Status != SipAccountStatus.Offline) * divertNode.QueueSizePerOnlineUsers))
                {
                    dialog.CurrentNode = dialog.Graph.FindNodeById(divertNode.FullQueueNode);
                    dialog.Extension = "FullQueue";
                    if (dialog.CurrentNode != null)
                    {
                        Logger.WriteInfo("Queue is full, current size: {0}, target node: {1}", DivertQueue.Count, dialog.CurrentNode.ID);
                        TrackCurrentNode(dialog);
                        return;
                    }
                }
            }
            #endregion

            dialog.Status = DialogStatus.WaitForDiverting;
            dialog.WaitForDivertingTimeout = divertNode.Timeout;

            if (string.IsNullOrWhiteSpace(Config.Default.DivertWaitGraph))
            {
                VoiceStream voiceStram = new VoiceStream();
                if (!string.IsNullOrEmpty(divertNode.AsDivertNode.WaitMessage))
                    voiceStram.AddVoice(new Guid(divertNode.AsDivertNode.WaitMessage));

                if (!string.IsNullOrEmpty(divertNode.AsDivertNode.WaitSound))
                    voiceStram.AddVoice(new Guid(divertNode.AsDivertNode.WaitSound));
                else
                    voiceStram.AddVoice(Constants.VoiceName_WaitMusic);

                SipServer.PlayVoice(dialog, voiceStram.stream.ToArray());
            }
            else
            {
                using (UmsDataContext dc = new UmsDataContext())
                {
                    var graphRecord = dc.Graphs.FirstOrDefault(g => g.ID == new Guid(Config.Default.DivertWaitGraph));
                    if (graphRecord == null)
                    {
                        Logger.WriteCritical("DivertWaitGraph ID:'{0}' not found", Config.Default.DivertWaitGraph);
                        return;
                    }

                    var graphXml = graphRecord.Data.ToString();
                    var graph = Schema.Graph.Deserialize(graphXml);

                    dialog.BeforeDivertGraph = dialog.Graph;
                    dialog.BeforeDivertNode = dialog.CurrentNode;

                    dialog.Graph = graph;
                    dialog.CurrentNodeID = graph.StartNode;
                    TrackCurrentNode(dialog);
                }
            }

            if (insertInQueueHead)
                DivertQueue.Insert(0, dialog);
            else
                DivertQueue.Add(dialog);

            //CheckDivertQueue(null);
        }

        void ExtractDivertTargetAccounts(SipDialog dialog, DivertNode divertNode)
        {
            var splittedPhones = divertNode.DynamicTargetPhone.Split(',');
            List<string> phones = new List<string>();
            foreach (var phone in splittedPhones)
            {
                if (Regex.IsMatch(phone, @"\d+-\d+"))
                {
                    int start = int.Parse(phone.Split('-').First());
                    int end = int.Parse(phone.Split('-').Last());

                    if (end >= start)
                        for (int index = start; index <= end; index++)
                            phones.Add(index.ToString());
                    else
                        for (int index = start; index >= end; index--)
                            phones.Add(index.ToString());
                }
                else
                {
                    phones.Add(phone);
                }
            }


            var registeredAccounts = SipServer.Accounts.Where(a => a.Status != SipAccountStatus.Offline);
            List<DivertTarget> divertTargets = new List<DivertTarget>();


            foreach (var phone in phones.Distinct())
            {
                foreach (var account in registeredAccounts)
                {
                    if (phone == account.UserID || (!String.IsNullOrEmpty(account.MatchRule) && Regex.IsMatch(phone, account.MatchRule)))
                    {
                        divertTargets.Add(new DivertTarget(account, phone));
                    }
                }
            }

            if (divertTargets.Count() == 0)
                return;

            divertTargets.Clear();
            foreach (var phone in phones.Distinct())
            {
                foreach (var account in SipServer.Accounts)
                {
                    if (phone == account.UserID || (!String.IsNullOrEmpty(account.MatchRule) && Regex.IsMatch(phone, account.MatchRule)))
                        divertTargets.Add(new DivertTarget(account, phone));
                }
            }

            divertTargets = divertTargets.OrderBy(a => a.Account.LastCallTime).ToList();
            dialog.DivertTargets = new SafeCollection<DivertTarget>(divertTargets);
            dialog.DivertTargets = new SafeCollection<DivertTarget>(divertTargets);

            Logger.WriteInfo("Create Operators Queue for dialog {0}: {1}", dialog.DialogID, string.Join(",", (from t in divertTargets select t.Account.UserID).ToArray()));
        }

        SafeCollection<SipDialog> DivertQueue = new SafeCollection<SipDialog>();
    }
}
